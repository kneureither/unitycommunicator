from unitycommunicator import UnityCommunicator
from PIL import Image

# Specify location of unity executable build
unity_build_path = 'TCPGeometricsBuild.app'

with UnityCommunicator(unity_build_path) as uc:
    json_data = uc.read_json_file('ParameterFiles/parameters_geometrics0.json')
    scene_img, scene_id = uc.render_parameters(json_data)
    Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(scene_id).zfill(3)))

    json_data = uc.read_json_file('ParameterFiles/parameters_geometrics1.json')
    scene_img, scene_id = uc.render_parameters(json_data)
    Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(scene_id).zfill(3)))

    json_data = uc.read_json_file('ParameterFiles/parameters_geometrics2.json')
    scene_img, scene_id = uc.render_parameters(json_data)
    Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(scene_id).zfill(3)))
