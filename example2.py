from unitycommunicator import UnityCommunicator
from PIL import Image

# Specify location of unity executable build
unity_build_path = 'UCGeometrics/Builds/UCGeometricsBuild.app'

#initialize UC class
UC = UnityCommunicator(unity_build_path, use_with_unity_build=True)


#Render the data
json_data = UC.read_json_file('ParameterFiles/parameters_geometrics0.json')
scene_img, meta_data_dict = UC.render_parameters(json_data)
Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(meta_data_dict['sceneID']).zfill(3)))

json_data = UC.read_json_file('ParameterFiles/parameters_geometrics1.json')
scene_img, meta_data_dict = UC.render_parameters(json_data)
Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(meta_data_dict['sceneID']).zfill(3)))

json_data = UC.read_json_file('ParameterFiles/parameters_geometrics2.json')
scene_img, meta_data_dict = UC.render_parameters(json_data)
Image.fromarray(scene_img).save('SavedScenes/Rendered_Scene_ID-{:3}.png'.format(str(meta_data_dict['sceneID']).zfill(3)))

#Close connection
UC.close()
