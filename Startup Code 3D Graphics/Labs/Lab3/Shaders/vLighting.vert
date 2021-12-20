#version 330

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform vec3 uLightPosition;


//uniform vec3 uLightDirection;


in vec3 vPosition; 
in vec3 vNormal;
out vec4 oColour;

void main() 
{
gl_Position = vec4(vPosition, 1) * uModel * uView * uProjection; 
vec3 inverseTransposeNormal = normalize(vNormal * mat3(transpose(inverse(uModel * uView))));
vec4 surfacePosition = vec4(vPosition, 1) * uModel * uView;
vec4 lightPosition = vec4(uLightPosition, 1) * uView;
vec4 lightDir = normalize(lightPosition - surfacePosition);
oColour = vec4(vec3(max(dot(vec4(inverseTransposeNormal, 1), lightDir), 0)), 1);
//L3T9 Implemented positional light



	
	//vec3 inverseTransposeNormal = normalize(vNormal * mat3(transpose(inverse(uModel * uView))));
	//vec3 lightDir = normalize(-uLightPosition * mat3(uView));
    //oColour = vec4(vec3(max(dot(inverseTransposeNormal, lightDir), 0)), 1);
	//L3T8 Adjusted directional light for a rotated view matrix








	//vec3 inverseTransposeNormal = normalize(vNormal * mat3(transpose(inverse(uModel * uView))));
   // oColour = vec4(vec3(max(dot(inverseTransposeNormal, -uLightDirection), 0)), 1);
	//L3T7 Adjusted normal vectors by multiplying by the inverse transpose of the model view matrix
}