xof 0302txt 0064
template Header {
 <3D82AB43-62DA-11cf-AB39-0020AF71E433>
 WORD major;
 WORD minor;
 DWORD flags;
}

template Vector {
 <3D82AB5E-62DA-11cf-AB39-0020AF71E433>
 FLOAT x;
 FLOAT y;
 FLOAT z;
}

template Coords2d {
 <F6F23F44-7686-11cf-8F52-0040333594A3>
 FLOAT u;
 FLOAT v;
}

template Matrix4x4 {
 <F6F23F45-7686-11cf-8F52-0040333594A3>
 array FLOAT matrix[16];
}

template ColorRGBA {
 <35FF44E0-6C7C-11cf-8F52-0040333594A3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
 FLOAT alpha;
}

template ColorRGB {
 <D3E16E81-7835-11cf-8F52-0040333594A3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
}

template IndexedColor {
 <1630B820-7842-11cf-8F52-0040333594A3>
 DWORD index;
 ColorRGBA indexColor;
}

template Boolean {
 <4885AE61-78E8-11cf-8F52-0040333594A3>
 WORD truefalse;
}

template Boolean2d {
 <4885AE63-78E8-11cf-8F52-0040333594A3>
 Boolean u;
 Boolean v;
}

template MaterialWrap {
 <4885AE60-78E8-11cf-8F52-0040333594A3>
 Boolean u;
 Boolean v;
}

template TextureFilename {
 <A42790E1-7810-11cf-8F52-0040333594A3>
 STRING filename;
}

template Material {
 <3D82AB4D-62DA-11cf-AB39-0020AF71E433>
 ColorRGBA faceColor;
 FLOAT power;
 ColorRGB specularColor;
 ColorRGB emissiveColor;
 [...]
}

template MeshFace {
 <3D82AB5F-62DA-11cf-AB39-0020AF71E433>
 DWORD nFaceVertexIndices;
 array DWORD faceVertexIndices[nFaceVertexIndices];
}

template MeshFaceWraps {
 <4885AE62-78E8-11cf-8F52-0040333594A3>
 DWORD nFaceWrapValues;
 Boolean2d faceWrapValues;
}

template MeshTextureCoords {
 <F6F23F40-7686-11cf-8F52-0040333594A3>
 DWORD nTextureCoords;
 array Coords2d textureCoords[nTextureCoords];
}

template MeshMaterialList {
 <F6F23F42-7686-11cf-8F52-0040333594A3>
 DWORD nMaterials;
 DWORD nFaceIndexes;
 array DWORD faceIndexes[nFaceIndexes];
 [Material]
}

template MeshNormals {
 <F6F23F43-7686-11cf-8F52-0040333594A3>
 DWORD nNormals;
 array Vector normals[nNormals];
 DWORD nFaceNormals;
 array MeshFace faceNormals[nFaceNormals];
}

template MeshVertexColors {
 <1630B821-7842-11cf-8F52-0040333594A3>
 DWORD nVertexColors;
 array IndexedColor vertexColors[nVertexColors];
}

template Mesh {
 <3D82AB44-62DA-11cf-AB39-0020AF71E433>
 DWORD nVertices;
 array Vector vertices[nVertices];
 DWORD nFaces;
 array MeshFace faces[nFaces];
 [...]
}

Header{
1;
0;
1;
}

Mesh {
 67;
 0.30281;0.01594;0.30281;,
 0.30891;0.01472;0.30281;,
 0.30281;0.01472;0.30891;,
 0.31408;0.01127;0.30281;,
 0.31078;0.01127;0.31078;,
 0.30281;0.01127;0.31408;,
 0.31754;0.00610;0.30281;,
 0.31556;0.00610;0.31017;,
 0.31017;0.00610;0.31556;,
 0.30281;0.00610;0.31754;,
 0.31875;0.00000;0.30281;,
 0.31754;0.00000;0.30891;,
 0.31408;0.00000;0.31408;,
 0.30891;0.00000;0.31754;,
 0.30281;0.00000;0.31875;,
 -0.30281;0.01472;0.30891;,
 -0.30891;0.01472;0.30281;,
 -0.30281;0.01594;0.30281;,
 -0.31078;0.01127;0.31078;,
 -0.31408;0.01127;0.30281;,
 -0.30281;0.01127;0.31408;,
 -0.31556;0.00610;0.31017;,
 -0.31754;0.00610;0.30281;,
 -0.31017;0.00610;0.31556;,
 -0.30281;0.00610;0.31754;,
 -0.31754;0.00000;0.30891;,
 -0.31875;0.00000;0.30281;,
 -0.31408;0.00000;0.31408;,
 -0.30891;0.00000;0.31754;,
 -0.30281;0.00000;0.31875;,
 0.30281;0.01472;-0.30891;,
 0.30891;0.01472;-0.30281;,
 0.30281;0.01594;-0.30281;,
 0.31078;0.01127;-0.31078;,
 0.31408;0.01127;-0.30281;,
 0.30281;0.01127;-0.31408;,
 0.31556;0.00610;-0.31017;,
 0.31754;0.00610;-0.30281;,
 0.31017;0.00610;-0.31556;,
 0.30281;0.00610;-0.31754;,
 0.31754;0.00000;-0.30891;,
 0.31875;0.00000;-0.30281;,
 0.31408;0.00000;-0.31408;,
 0.30891;0.00000;-0.31754;,
 0.30281;0.00000;-0.31875;,
 -0.30281;0.01594;-0.30281;,
 -0.30891;0.01472;-0.30281;,
 -0.30281;0.01472;-0.30891;,
 -0.31408;0.01127;-0.30281;,
 -0.31078;0.01127;-0.31078;,
 -0.30281;0.01127;-0.31408;,
 -0.31754;0.00610;-0.30281;,
 -0.31556;0.00610;-0.31017;,
 -0.31017;0.00610;-0.31556;,
 -0.30281;0.00610;-0.31754;,
 -0.31875;0.00000;-0.30281;,
 -0.31754;0.00000;-0.30891;,
 -0.31408;0.00000;-0.31408;,
 -0.30891;0.00000;-0.31754;,
 -0.30281;0.00000;-0.31875;,
 0.15348;0.02157;0.14821;,
 0.30281;0.01594;-0.30281;,
 0.30281;0.01594;0.30281;,
 -0.15348;0.02157;0.14821;,
 -0.30281;0.01594;0.30281;,
 0.15348;0.02157;-0.15461;,
 -0.15348;0.02157;-0.15461;;
 
 104;
 3;2,1,0;,
 3;4,3,1;,
 3;1,2,4;,
 3;5,4,2;,
 3;7,6,3;,
 3;3,4,7;,
 3;8,7,4;,
 3;4,5,8;,
 3;9,8,5;,
 3;11,10,6;,
 3;6,7,11;,
 3;12,11,7;,
 3;7,8,12;,
 3;13,12,8;,
 3;8,9,13;,
 3;14,13,9;,
 3;17,16,15;,
 3;16,19,18;,
 3;18,15,16;,
 3;15,18,20;,
 3;19,22,21;,
 3;21,18,19;,
 3;18,21,23;,
 3;23,20,18;,
 3;20,23,24;,
 3;22,26,25;,
 3;25,21,22;,
 3;21,25,27;,
 3;27,23,21;,
 3;23,27,28;,
 3;28,24,23;,
 3;24,28,29;,
 3;32,31,30;,
 3;31,34,33;,
 3;33,30,31;,
 3;30,33,35;,
 3;34,37,36;,
 3;36,33,34;,
 3;33,36,38;,
 3;38,35,33;,
 3;35,38,39;,
 3;37,41,40;,
 3;40,36,37;,
 3;36,40,42;,
 3;42,38,36;,
 3;38,42,43;,
 3;43,39,38;,
 3;39,43,44;,
 3;47,46,45;,
 3;49,48,46;,
 3;46,47,49;,
 3;50,49,47;,
 3;52,51,48;,
 3;48,49,52;,
 3;53,52,49;,
 3;49,50,53;,
 3;54,53,50;,
 3;56,55,51;,
 3;51,52,56;,
 3;57,56,52;,
 3;52,53,57;,
 3;58,57,53;,
 3;53,54,58;,
 3;59,58,54;,
 3;15,2,0;,
 3;17,15,0;,
 3;20,5,2;,
 3;15,20,2;,
 3;24,9,5;,
 3;20,24,5;,
 3;29,14,9;,
 3;24,29,9;,
 3;45,32,30;,
 3;47,45,30;,
 3;47,30,35;,
 3;50,47,35;,
 3;50,35,39;,
 3;54,50,39;,
 3;54,39,44;,
 3;59,54,44;,
 3;1,31,32;,
 3;0,1,32;,
 3;3,34,31;,
 3;1,3,31;,
 3;6,37,34;,
 3;3,6,34;,
 3;10,41,37;,
 3;6,10,37;,
 3;46,16,17;,
 3;45,46,17;,
 3;48,19,16;,
 3;46,48,16;,
 3;51,22,19;,
 3;48,51,19;,
 3;55,26,22;,
 3;51,55,22;,
 3;62,61,60;,
 3;45,64,63;,
 3;60,61,65;,
 3;45,63,66;,
 3;64,62,60;,
 3;63,64,60;,
 3;66,65,61;,
 3;45,66,61;;
 
 MeshMaterialList {
  1;
  104;
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0;;
  Material {
   1.000000;1.000000;1.000000;1.000000;;
   0.000000;
   0.000000;0.000000;0.000000;;
   0.000000;0.000000;0.000000;;
  }
 }
 MeshNormals {
  64;
  -0.066501;0.993791;-0.089207;,
  0.082717;0.994871;-0.058227;,
  -0.089167;0.993819;0.066132;,
  0.071290;0.992034;0.103858;,
  -0.064093;0.194696;0.978767;,
  0.096190;0.194193;0.976236;,
  -0.096190;0.194193;-0.976236;,
  0.064093;0.194696;-0.978767;,
  0.976236;0.194193;-0.096190;,
  0.978767;0.194696;0.064093;,
  -0.978767;0.194696;-0.064093;,
  -0.976236;0.194193;0.096190;,
  -0.124620;0.932077;0.340152;,
  0.124696;0.907147;0.401914;,
  -0.126204;0.717213;0.685331;,
  0.126113;0.669622;0.731917;,
  -0.112551;0.390897;0.913527;,
  0.112389;0.328904;0.937652;,
  -0.124696;0.907147;-0.401914;,
  0.124620;0.932077;-0.340152;,
  -0.126113;0.669622;-0.731917;,
  0.126204;0.717213;-0.685331;,
  -0.112389;0.328904;-0.937652;,
  0.112551;0.390897;-0.913527;,
  0.401914;0.907147;-0.124696;,
  0.340152;0.932077;0.124620;,
  0.731917;0.669622;-0.126113;,
  0.685331;0.717213;0.126204;,
  0.937652;0.328904;-0.112389;,
  0.913527;0.390897;0.112551;,
  -0.340152;0.932077;-0.124620;,
  -0.401914;0.907147;0.124696;,
  -0.685331;0.717213;-0.126204;,
  -0.731917;0.669622;0.126113;,
  -0.913527;0.390897;-0.112551;,
  -0.937652;0.328904;0.112389;,
  0.335431;0.201069;0.920357;,
  0.693624;0.194345;0.693624;,
  0.920357;0.201069;0.335431;,
  0.335431;0.201069;-0.920357;,
  0.693624;0.194345;-0.693624;,
  0.920357;0.201069;-0.335431;,
  -0.335431;0.201069;0.920357;,
  -0.693624;0.194345;0.693624;,
  -0.920357;0.201069;0.335431;,
  -0.335431;0.201069;-0.920357;,
  -0.693624;0.194345;-0.693624;,
  -0.920357;0.201069;-0.335431;,
  0.472235;0.744304;0.472235;,
  0.794186;0.405789;0.452332;,
  0.452332;0.405789;0.794185;,
  -0.472235;0.744304;0.472235;,
  -0.794186;0.405789;0.452332;,
  -0.452332;0.405789;0.794185;,
  0.472235;0.744304;-0.472235;,
  0.794186;0.405789;-0.452332;,
  0.452332;0.405789;-0.794185;,
  -0.472235;0.744304;-0.472235;,
  -0.794186;0.405789;-0.452332;,
  -0.452332;0.405789;-0.794185;,
  0.018839;0.999657;0.018198;,
  -0.025118;0.999611;0.012131;,
  0.018839;0.999642;-0.018984;,
  -0.012559;0.999601;-0.025311;;
  104;
  3;13,25,3;,
  3;48,27,25;,
  3;25,13,48;,
  3;15,48,13;,
  3;49,29,27;,
  3;27,48,49;,
  3;50,49,48;,
  3;48,15,50;,
  3;17,50,15;,
  3;38,9,29;,
  3;29,49,38;,
  3;37,38,49;,
  3;49,50,37;,
  3;36,37,50;,
  3;50,17,36;,
  3;5,36,17;,
  3;2,31,12;,
  3;31,33,51;,
  3;51,12,31;,
  3;12,51,14;,
  3;33,35,52;,
  3;52,51,33;,
  3;51,52,53;,
  3;53,14,51;,
  3;14,53,16;,
  3;35,11,44;,
  3;44,52,35;,
  3;52,44,43;,
  3;43,53,52;,
  3;53,43,42;,
  3;42,16,53;,
  3;16,42,4;,
  3;1,24,19;,
  3;24,26,54;,
  3;54,19,24;,
  3;19,54,21;,
  3;26,28,55;,
  3;55,54,26;,
  3;54,55,56;,
  3;56,21,54;,
  3;21,56,23;,
  3;28,8,41;,
  3;41,55,28;,
  3;55,41,40;,
  3;40,56,55;,
  3;56,40,39;,
  3;39,23,56;,
  3;23,39,7;,
  3;18,30,0;,
  3;57,32,30;,
  3;30,18,57;,
  3;20,57,18;,
  3;58,34,32;,
  3;32,57,58;,
  3;59,58,57;,
  3;57,20,59;,
  3;22,59,20;,
  3;47,10,34;,
  3;34,58,47;,
  3;46,47,58;,
  3;58,59,46;,
  3;45,46,59;,
  3;59,22,45;,
  3;6,45,22;,
  3;12,13,3;,
  3;2,12,3;,
  3;14,15,13;,
  3;12,14,13;,
  3;16,17,15;,
  3;14,16,15;,
  3;4,5,17;,
  3;16,4,17;,
  3;0,1,19;,
  3;18,0,19;,
  3;18,19,21;,
  3;20,18,21;,
  3;20,21,23;,
  3;22,20,23;,
  3;22,23,7;,
  3;6,22,7;,
  3;25,24,1;,
  3;3,25,1;,
  3;27,26,24;,
  3;25,27,24;,
  3;29,28,26;,
  3;27,29,26;,
  3;9,8,28;,
  3;29,9,28;,
  3;30,31,2;,
  3;0,30,2;,
  3;32,33,31;,
  3;30,32,31;,
  3;34,35,33;,
  3;32,34,33;,
  3;10,11,35;,
  3;34,10,35;,
  3;3,1,60;,
  3;0,2,61;,
  3;60,1,62;,
  3;0,61,63;,
  3;2,3,60;,
  3;61,2,60;,
  3;63,62,1;,
  3;0,63,1;;
 }
 MeshTextureCoords {
  67;
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.000000;0.000000;,
  0.750854;0.751163;,
  1.000000;0.000000;,
  1.000000;1.000000;,
  0.248837;0.751163;,
  0.000000;1.000000;,
  0.743928;0.256072;,
  0.255754;0.256072;;
 }
}