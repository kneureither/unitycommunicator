import json


if __name__ == '__main__':
    filename = 'parameters_geometrics_simple.json'
    jsonstring = open(filename).read()
    jsondata = json.loads(jsonstring)

    print(jsonstring)

    jsondumped = json.dumps(jsondata, separators = (',', ':'), indent=4)

    print('dumped data: ' + jsondumped)
