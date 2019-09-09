import UnityCommunicator

with UnityCommunicator() as uc:
    jsondata = uc.readJsonFile('ParameterFiles/parameters_geometrics0.json')
    img, sceneID = uc.renderParameters(jsondata)
    Image.fromarray(img).save('SavedScenes/FinalPictureID-' + str(sceneID) + '.png')

    jsondata = uc.readJsonFile('ParameterFiles/parameters_geometrics1.json')
    img, sceneID = uc.renderParameters(jsondata)
    Image.fromarray(img).save('SavedScenes/FinalPictureID-' + str(sceneID) + '.png')

    jsondata = uc.readJsonFile('ParameterFiles/parameters_geometrics2.json')
    img, sceneID = uc.renderParameters(jsondata)
    Image.fromarray(img).save('SavedScenes/FinalPictureID-' + str(sceneID) + '.png')
