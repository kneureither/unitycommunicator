import socket
import json
import logging
import numpy as np
from PIL import Image
import io


class UnityCommunicator:

    def __init__(self):
        self.logger = logging.getLogger('unity-com')
        self.logger.setLevel(logging.DEBUG)
        self.fh = logging.FileHandler('LOG/unity-communicator.log')
        self.fh.setLevel(logging.DEBUG)
        self.logger.addHandler(self.fh)
        self.formatter = logging.Formatter('%(asctime)s - %(name)s - %(levelname)s : %(message)s')
        self.fh.setFormatter(self.formatter)

        #Clear log at startup if it is longer than 1000 lines
        with f as open('LOG/unity-communicator.log', 'r'):
            loglength = len(f.readlines())

        if loglength > 1000:
            with open('LOG/unity-communicator.log', 'w'):
                pass


        self.logger.info('executing unity response server...')

        self.socket_timeout = 0.1
        self.path = '/Users/KonstantinN/OneDrive/Dokumente/1_STUDIUM/_2019-SS/INFAP/Unity/TCPGeometrics/Assets/StreamingAssets/'

        self.conn_unity = self._setup_server()

        ## TODO: Vielleicht hier noch standard konfiguration als parameter0 schicken

    def __enter__(self):
        return self

    def __exit__(self, type, value, traceback):
        self._sendData('END.')
        self.logger.debug('end command sent')
        self.conn_unity.close()

    def _setup_server(self):

        try:
            with open(self.path + 'tcpconfig.json', 'r') as f:
                config = json.load(f)
                f.close()
        except FileNotFoundError as e:
            self.logger.error('tcpconfig.json can not be found')
            print('ERROR: tcpconfig.json no such file, should be found in <UnityProject>/Assets/StreamingAssets/')
            raise e

        self.logger.info("CONFIG : " + str(config['ports'][1]) + " at " + str(config['host']))

        while 1:
            try:
                socket_unity = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
                socket_unity.bind((config['host'], config['ports'][1]))
            except socket.error as e:
                print(e)
                print('STATUS : try next port...')

                if (config['ports'][1]) >= 50100:
                    config['ports'][1] = 50000
                else:
                    config['ports'][1] += 1

                socket_unity.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

                with open(self.path + 'tcpconfig.json', 'w') as f:
                    json.dump(config, f)
                    f.close()
            else:
                break

        socket_unity = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        socket_unity.bind((config['host'], config['ports'][1]))

        socket_unity.listen(1)
        print('STATUS : waiting for connection from Unity at port {} ...'.format(config['ports'][1]))
        conn_unity, addr_unity = socket_unity.accept()
        print('STATUS : connection to Unity established at addr: ', str(addr_unity))

        conn_unity.settimeout(self.socket_timeout)

        return conn_unity

    def _sendData(self, message):
        """sends data to socket and adds end tag. End tag is then interpreted as end of message by unity"""
        self.logger.debug('_sendData(): entered')
        self.conn_unity.sendall((message + 'eod.').encode())  # End of Data
        self.logger.debug('_sendData(): sendall succeeded')


    def _receiveDataAsBytes(self):
        """receives data at the classes socket. ends by timeout, returns string object"""
        data_complete = bytearray([0])
        self.logger.debug('_receiveDataAsBytes(): entered')

        while 1:
            try:
                data = self.conn_unity.recv(1024)
            except socket.timeout:
                self.logger.error('_receiveDataAsBytes(): timeout -> exit _receiveData()')
                break

            data_complete = data_complete + data
            if not data:
                self.logger.debug('_receiveDataAsBytes(): no data anymore. receive ends.')
                break

        return data_complete


    def readJsonFile(self, filename):
        """loads json file as python directory"""
        jsonstring = open(filename).read()
        jsondata = json.loads(jsonstring)

        return jsondata

    def renderParameters(self, json_dict):
        """delivers given parameters as json string to unity and returns ID, status and np.array RGBA (height, width, 4)"""
        print("Entered renderParameterDict...")

        self.logger.debug('entered renderParameterDict()')

        json_string = json.dumps(json_dict, separators = (',', ':'))

        self.logger.info('start sending data...')
        self._sendData(json_string)
        self.logger.info('success: data sent.')

        while 1:
            unity_resp_bytes = self._receiveDataAsBytes()

            if unity_resp_bytes[-8:] == bytearray([255,0,250,251,252,253,254,255]):
                self.logger.debug('_renderParameters(): end tag from Unity detected, end receive')
                break

            self.logger.debug('run again _receiveData(): data so far ' + str(unity_resp_bytes))


        self.logger.debug('received scene of ' + str(len(unity_resp_bytes)) + ' bytes')

        metaLength = int.from_bytes(unity_resp_bytes[-12:-8], byteorder='little')
        metaBytes = unity_resp_bytes[-(12 + metaLength):-12].decode()
        metaData = json.loads(metaBytes)
        sceneID = metaData['sceneID']
        message = metaData['message']
        img_bytes = unity_resp_bytes[1:-(12+metaLength)]

        self.logger.info('unpacked scene with ID ' + str(sceneID))

        print(message)
        pilImg = Image.open(io.BytesIO(img_bytes))
        img = np.array(pilImg)

        print("response status from Unity: ID " + str(sceneID) + " received")

        return img, sceneID

        def renderParameterFile(self, json_file):
            jsondata = self.readJsonFile(jsonfile)
            return renderParameters(self, jsondata)


if __name__ == '__main__':

    with UnityCommunicator() as uc:
        jsondata = uc.readJsonFile('ParameterFiles/parameters_geometrics0_arraytest.json')
        img, sceneID = uc.renderParameters(jsondata)
        Image.fromarray(img).save('SavedScenes/FinalPictureID-' + str(sceneID) + '.png')

        jsondata = uc.readJsonFile('ParameterFiles/parameters_geometrics1.json')
        img, sceneID = uc.renderParameters(jsondata)
        Image.fromarray(img).save('SavedScenes/FinalPictureID-' + str(sceneID) + '.png')

        jsondata = uc.readJsonFile('ParameterFiles/parameters_geometrics2.json')
        img, sceneID = uc.renderParameters(jsondata)
        Image.fromarray(img).save('SavedScenes/FinalPictureID-' + str(sceneID) + '.png')
