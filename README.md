# WPF Kinect for RULA Calculation
This my Kinect for RULA (Raipd Upper Limb Assessment) Calculate using C# and WPF App.
Purpose of this application is for calculating body joint using kinect. after that application will transform the data into a RULA score.

# Citate
I am got inspired to do this for my thesis is from this Ergonomic Journal. Here is the DOI, so you can know what i am trying to do here
 - https://doi.org/10.1016/j.apergo.2017.02.015

# Here some of my reference
reference for calculating angle but on different condition, example: Upper arm must be calculate in Saggital Plane.
1. https://stackoverflow.com/a/39673693/6759373
2. https://stackoverflow.com/a/25546387/6759373
3. https://math.stackexchange.com/questions/361412/finding-the-angle-between-three-points

# Body Drawing Reference
- https://mtaulty.com/2014/08/08/m_15354/
- https://www.codeproject.com/articles/743862/kinect-for-windows-version-body-tracking

# This Reference is for smoothing data that received by Kinect 
- https://social.msdn.microsoft.com/Forums/en-US/850b61ce-a1f4-4e05-a0c9-b0c208276bec/joint-smoothing-code-for-c?forum=kinectv2sdk

# Kalman Filter
1. https://wangready.wordpress.com/2013/05/02/kalman-filter/
2. https://piptools.net/algoritma-kalman-filter/
3. https://github.com/KonstantinosAng/Kinect2-Kalman/blob/master/depth_kalman_kinectv2.m
4. http://bilgin.esme.org/BitsAndBytes/KalmanFilterforDummies
5. https://docplayer.info/40536055-Pemanfaatan-filter-dalam-peningkatan-hasil-skeletal-tracking-pada-kinect.html (Erick Pranata Thesis for my kalman tunning by using variance of the body every 5 frame)

# Some pict from my App
![My Image](https://github.com/glovenkevin/img/blob/master/KV2RULA-v1.0.png?raw=true)
