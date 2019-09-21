import socket
import json
import logging
import numpy as np
from PIL import Image
import io
import os


class UnityCommunicator:
    """provides all methods for communication to unity game enginge.

    Scene parameters can be sent to unity, the corresponding scene can be sent back. Therefore a tcp connection is used.
    The tcp parameters such as 'host' and 'port' are specified in tcpconfig.json file in the unity projects 'StreamingAssets'
    folder.

    See also
    --------
    Documentation 'unitycommunicator'
    Created during software practical at Heidelberg Collaboratory of image processing. September 2019
    https://hciweb.iwr.uni-heidelberg.de/compvis


    Examples
    --------
    Render a scene parameter set (from json dictionary which can be passed directly or read from file).

    >>> from unitycommunicator import UnityCommunicator

    >>> with UnityCommunicator('unity_build.app', use_with_unity_build=True) as uc:
    >>>     json_data = uc.read_json_file('parameterfile.json')
    >>>     scene_img, scene_id = uc.render_parameters(json_data)

    To save image, use e.g. pillow

    >>> from PIL import Image
    >>> Image.fromarray(scene_img).save('Rendered_Scene_ID-{:3}.png'.format(str(scene_id).zfill(3)))
    >>> # str.zfill ensures a consistent file naming (leading zeros)

    """

    def __init__(self, unity_build_path, use_with_unity_build, loglevel=logging.WARNING):
        """Sets up everything to enable a tcp connection to unity.

        Constructor of class, stars logging, stars unity build executable, starts tcp socket.

        Parameters
        ---------
        unity_build_path : str
            path to unity build executable

        use_with_unity_build : bool
            if used with unity engine True, if used with unity build False
            Changes some path specifications and also starts the build if True
        loglevel : logging.<LEVEL>
            Specify loglevel of console prompts

        """

        self.log_path = 'log/unity-communicator.log'

        # Create logger
        self.logger = logging.getLogger('UClog')
        self.logger.setLevel(logging.DEBUG)
        # Create console handler
        self.ch = logging.StreamHandler()
        self.ch.setLevel(logging.INFO)
        # Create file handler
        self.fh = logging.FileHandler(self.log_path)
        self.fh.setLevel(logging.DEBUG)
        # Add formatter
        self.formatter_fh = logging.Formatter('%(asctime)s - %(levelname)s : %(message)s')
        self.fh.setFormatter(self.formatter_fh)
        self.formatter_ch = logging.Formatter('%(levelname)s : %(message)s')
        self.ch.setFormatter(self.formatter_ch)
        # Add fh and ch to logger
        self.logger.addHandler(self.fh)
        self.logger.addHandler(self.ch)

        #Clear log at startup if it is longer than 1000 lines
        with open(self.log_path, 'r') as f:
            log_length = len(f.readlines())

        if log_length > 1000:
            with open(self.log_path, 'w'):
                pass

        print('')
        self.logger.info('starting unity communication server...\n')


        self.unity_build_path = unity_build_path

        if use_with_unity_build == True:
            # For execution with BUILD
            # Start unity build
            os.system('open ' + self.unity_build_path)
            self.streaming_assets_path = self.unity_build_path + '/Contents/Resources/Data/StreamingAssets/'
        else:
            # For execution with unity engine
            self.streaming_assets_path = self.unity_build_path + '/Assets/StreamingAssets/'
            self.logger.warning('Please start Unity...')

        # Specify paths to tcpconfig.json file (in streamingAssets folder of unity project
        # self.streaming_assets_path = '/Users/KonstantinN/OneDrive/Dokumente/1_STUDIUM/_2019-SS/INFAP/Unity/TCPGeometrics/Assets/StreamingAssets/'
        self.tcp_config_path = self.streaming_assets_path + 'tcpconfig.json'

        # Timeout for socket connection, will be used to prevent lock (in case both peers are receiving)
        self.socket_timeout = 0.1

        # Call function for tcp server setup
        self.conn_unity = self._setup_server()


    def __enter__(self):
        """Necessary for usage in with...as statement.

        Returns
        -------
        self : UnityCommunicator
           For use in "with UnityCommunicator(unity_build_path) as:"
        """

        return self

    def __exit__(self, type, value, traceback):
        """sends end request to Unity, closes TCP connection. Called when used in with statement"""

        self._send_data('END.')
        self.logger.warning('End command was sent to Unity.')
        self.conn_unity.close()
        self.logger.warning('Socket closed. exit...')

    def _setup_server(self):
        """Sets up TCP server for unity connection.

        Therefore it looks for a free port in range (50000, 50099) and stores
        the used port in tcpconfig.json file of Unity project at /StreamingAssets/tcpconfig.json.
        Unity reads the port and accepts the connection.

        Returns
        --------
        socket.socket
                the socket connection to unity

        Raises
        ------
        FileNotFoundError
                tcpconfig.json in StreamingAssets Folder of UnityProject can not be found
        socket.error
                to catch if socket is in use
        """

        # Open tcpconfig.json file
        try:
            with open(self.tcp_config_path, 'r') as f:
                config = json.load(f)
                f.close()
        except FileNotFoundError as e:
            self.logger.error('\'tcpconfig.json\' no such file, should be found in <UnityProject>/Assets/StreamingAssets/')
            raise e


        # try to bind port specified in tcpconfig.json file
        while 1:
            try:
                socket_unity = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
                socket_unity.bind((config['host'], config['ports'][1]))
            except socket.error as e:
                # If port is in use, try next port
                print(e)
                self.logger.warning('try next port...')

                if (config['ports'][1]) >= 50100:
                    config['ports'][1] = 50000
                else:
                    config['ports'][1] += 1
            else:
                break

        # Write used port to 'tcpconfig.json'
        with open(self.tcp_config_path, 'w') as f:
            json.dump(config, f)
            f.close()


        socket_unity = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        socket_unity.bind((config['host'], config['ports'][1]))

        socket_unity.listen(1)
        self.logger.warning('waiting for connection from Unity at port {} ...'.format(config['ports'][1]))
        conn_unity, addr_unity = socket_unity.accept()
        self.logger.warning('connection to Unity established at addr: ' + str(addr_unity) + '\n')

        conn_unity.settimeout(self.socket_timeout)

        return conn_unity


    def _send_data(self, message):
        """sends string to socket. 
        
        At the end of each message, an end tag 'eod.' is added to the message.
        Unity detects the end of the message by reading this tag.
        
        Parameters
        ---------
        message : str
            server message to unity. should be in json format.
        
        """
        
        self.conn_unity.sendall((message + 'eod.').encode())  # End of Data
        self.logger.debug('_send_data(): sendall succeeded')


    def _receive_data_as_bytes(self):
        """receives data at the classes socket.
        
        _receive_data_as_bytes is either ended by timeout or if not data is sent.

        Returns
        -------
        data_complete : bytearray
            bytearray of message packages
        """

        self.logger.debug('_receive_data_as_bytes(): entered')
        data_complete = bytearray([0])

        while 1:
            try:
                data = self.conn_unity.recv(1024)
            except socket.timeout:
                self.logger.debug('_receive_data_as_bytes(): timeout -> exit _receiveData()')
                break

            data_complete = data_complete + data
            if not data:
                self.logger.debug('_receive_data_as_bytes(): no data anymore. receive ends.')
                break

        return data_complete


    def read_json_file(self, file_name):
        """loads json file as python directory.

        Parameters
        ---------
        filename : str
            path to parameter file

        Returns
        -------
        dictionary
            contains json file parameters. Returns None if invalid file or parsing to dict impossible.
        """

        try:
            json_string = open(file_name).read()
        except FileNotFoundError as e:
            self.logger.error(file_name + ' can not be found. returned None')
            #Return none, in order to skip this parameter file. (None check is implemented in render_parameters())
            return None

        try:
            json_dict = json.loads(json_string)
        except json.decoder.JSONDecodeError as e:
            self.logger.error(file_name + ' can not be converted to json. Returned None')
            # Return none, in order to skip this parameter file. (None check is implemented in render_parameters())
            return None

        return json_dict

    def render_parameters(self, param_dict):
        """controls the rendering of one scene parameter set in unity.

        Therefore it delivers given parameters as json string to unity and returns the scene meta data as well as a
        rendered scene picture. The json_dict should contain parameters for every scene object in unity. Also the class
        'JSONCaptureParameters' in Assets/Scripts/ in unity should contain a representation for the delivered dict.

        Parameters
        ---------
        param_dict : dictionary
            should contain all parameters of scene objects in unity.

        Returns
        -------
        np.array
            contains captured scene as RGBA array of dimensions (height, width, 4)
        int
            the sceneID specified in the deliverd param_dict for the scene

        See also
        --------
        Documentation unitycommunicator.

        Examples
        --------
        See UnityCommunicator().__doc__

        """

        self.logger.info('Render parameters of scene with ID ' + str(param_dict['sceneID']) + ' ...')

        if param_dict is None:
            self.logger.error('render_parameters(): Aborted because of invalid json_dict')
            return None

        json_string = json.dumps(param_dict, separators=(',', ':'))

        self.logger.debug('render_parameters(): Start sending data...')
        self._send_data(message=json_string)
        self.logger.debug('render_parameters(): Success! Data sent.')

        # Run the _receive_data_as_bytes() until valid response (bytearray end tag).
        # This uses the socket timeout break in _receive_data_as_bytes() to wait for the rendering to finish.
        # This is necessary to avoid a lock, which occurs when python starts to receive before unity sends.
        while 1:
            unity_resp_bytes = self._receive_data_as_bytes()

            if unity_resp_bytes[-8:] == bytearray([255,0,250,251,252,253,254,255]):
                self.logger.debug('render_parameters(): End tag from Unity detected, end receive')
                break

            self.logger.debug('render_parameters(): Run again receive_data()')

        self.logger.debug('render_parameters(): Received scene of ' + str(len(unity_resp_bytes)) + ' bytes')

        # Load the meta data file in dictionary and process the scene_img
        meta_length = int.from_bytes(unity_resp_bytes[-12:-8], byteorder='little')
        meta_bytes = unity_resp_bytes[-(12 + meta_length):-12].decode()
        meta_data_dict = json.loads(meta_bytes)
        scene_id = meta_data_dict['sceneID']
        message = meta_data_dict['message']
        img_bytes = unity_resp_bytes[1:-(12+meta_length)]

        self.logger.info('Received and unpacked scene with ID ' + str(scene_id))
        self.logger.info('Scene meta data message: ' + message + '\n')

        pillow_img = Image.open(io.BytesIO(img_bytes))
        scene_img = np.array(pillow_img)

        return scene_img, scene_id


if __name__ == '__main__':
    with UnityCommunicator('/Users/KonstantinN/OneDrive/Dokumente/1_STUDIUM/_2019-SS/INFAP/Unity/TCPGeometrics', use_with_unity_build=False, loglevel=logging.INFO) as uc:
        json_data = uc.read_json_file('ParameterFiles/parameters_geometrics0.json')
        scene_img, scene_id = uc.render_parameters(json_data)
        Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(scene_id).zfill(3)))

        json_data = uc.read_json_file('ParameterFiles/parameters_geometrics1.json')
        scene_img, scene_id = uc.render_parameters(json_data)
        Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(scene_id).zfill(3)))

        json_data = uc.read_json_file('ParameterFiles/parameters_geometrics2.json')
        scene_img, scene_id = uc.render_parameters(json_data)
        Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(scene_id).zfill(3)))
