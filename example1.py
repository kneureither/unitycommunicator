from unitycommunicator import UnityCommunicator
from PIL import Image

# Specify location of unity executable build
unity_build_path = 'UCGeometrics/Builds/UCGeometricsBuild.app'

with UnityCommunicator(unity_build_path, use_with_unity_build=True, width=800, height=550) as uc:
    json_data = uc.read_json_file('ParameterFiles/parameters_geometrics0.json')
    scene_img, meta_data_dict = uc.render_parameters(json_data)
    Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(meta_data_dict['sceneID']).zfill(3)))

    json_data = uc.read_json_file('ParameterFiles/parameters_geometrics1.json')
    scene_img, meta_data_dict = uc.render_parameters(json_data)
    Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(meta_data_dict['sceneID']).zfill(3)))

    json_data = uc.read_json_file('ParameterFiles/parameters_geometrics2.json')
    scene_img, meta_data_dict = uc.render_parameters(json_data)
    Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(meta_data_dict['sceneID']).zfill(3)))
