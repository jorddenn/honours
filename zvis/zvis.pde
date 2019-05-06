Table z;

int x = 0, y = 0;
int minBright = MAX_INT, maxBright = 0;
int angle = 0;

float orbitRadius= 2000;
float ypos = height / 2, xpos, zpos;

void setup() {
  size(1,1, P3D);
  surface.setResizable(true);
  z = loadTable("depth.csv");
    
  x = z.getColumnCount();
  y = z.getRowCount();
  
  noSmooth();
  noStroke();
  
  for (int i = 0; i < x; i++){
    for (int r = 1; r < y; r++){
      if (z.getInt(r, i) < minBright && z.getInt(r, i) != 0) minBright = z.getInt(r, i);
      if (z.getInt(r, i) > maxBright) maxBright = z.getInt(r, i);
    }
  }

  println(minBright);
  println(maxBright);
  
  sphereDetail(3);
  
  surface.setSize(x * 2, y * 2);
}

void draw(){
  background(0);
  
  xpos= cos(angle / 6)*orbitRadius;
  zpos= sin(angle / 6)*orbitRadius;
  
  camera(xpos, ypos, zpos, width / 2, height / 2, 0, 0, 1, 0);
  
  angle = (angle + 1) % 360;
  
  for(int i = 0; i < x; i++){
    for(int r = 0; r < y; r++){
      
      fill(map(z.getInt(r, i), minBright, maxBright, 255, 0));
      if(z.getInt(r, i) == 0){
        fill(0);
      }
      
      pushMatrix();
      translate(i * 2, r * 2, map(z.getInt(r, i), 0, maxBright, -1.3, -0.3));
      //rotateY(sin(angle));
      sphere(2);
      popMatrix();
    }
  }
  
}
