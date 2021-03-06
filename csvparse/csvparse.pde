import controlP5.*;
Table in, out, z, inZ;

String file = "image1a";

float maxX = 0, maxY = 0, maxZ;
float minX = MAX_FLOAT, minY = MAX_FLOAT, minZ = MAX_FLOAT;
int x = 0, y = 0;

PImage a, im;

ControlP5 p5;

float xdiv = 1290;
float ydiv = 869;

void setup() {
  size(1,1);
  surface.setResizable(true);
  
  p5 = new ControlP5(this);
  p5.addSlider("x", 1, 2000, 1290, 0, 100, 200, 25);
  p5.addSlider("y", 1, 2000, 869, 0, 50, 200, 25);

  a = loadImage("image1a.JPG");
  im = loadImage("image.jpg");
  
  surface.setSize(im.width, im.height);
  
  image(im, 0,0);
  tint(255, 128);
  image(a, 400, 129, (float)a.width / ((float)a.width / (float)xdiv), (float)a.height / ((float)a.height / (float)ydiv));
  

  noTint();
  
  stroke(255);
  
  
  
}

void csv(){
in = loadTable(file + ".csv");
  out = new Table();
  inZ = loadTable("depth.csv");

  for (int r = 0; r < in.getRowCount(); r +=2) {
    for (int c = 0; c < in.getColumnCount(); c++) {
      if (in.getFloat(r + 1, c) > maxX) {
        maxX = in.getFloat(r + 1, c);
      }
      if (in.getFloat(r, c) > maxY) {
        maxY = in.getFloat(r, c);
      }
      if (in.getFloat(r + 1, c) < minX) {
        minX = in.getFloat(r + 1, c);
      }
      if (in.getFloat(r, c) < minY) {
        minY = in.getFloat(r, c);
      }
    }
  }
  

  for (int i = 0; i < in.getColumnCount(); i++) {
    out.addColumn();
  }

  int outRow = 0;

  for (int r = 0; r < in.getRowCount(); r += 2) {
    out.addRow();
    out.addRow();
    out.addRow();
    for (int c = 0; c < in.getColumnCount(); c++) {
      out.setFloat(outRow, c, map(in.getFloat(r + 1, c), minX, maxX, 1.0, -1.1));
      out.setFloat(outRow + 1, c, map(in.getFloat(r, c), minY, maxY, 1.3, -3));
      out.setFloat(outRow + 2, c, checkz(in.getFloat(r + 1, c), in.getFloat(r, c)));
      //out.setFloat(outRow + 2, c, 1.2);
    }
    outRow += 3;
  }
  
  mapZ();

  println(in.getColumnCount(), in.getRowCount());
  println(out.getColumnCount(), out.getRowCount());

  

  saveTable(out, file + "zz.csv");
}

float checkz(float x, float y){
  x = map(x, 0, a.width, 400, 1690);
  y = map(y, 0, a.height, 129, 998);
  
  /*
  x = map(x, 400, 1690, 0, im.width);
  x = map(x, 129, 998, 0, im.height);
  */
  //x = map(x, 0, im.width, 0, inZ.getColumnCount());
  //y = map(y, 0, im.height, 0, inZ.getRowCount());
  
  
  x = x * ((float)inZ.getColumnCount() / (float)im.width) - 61;
  y = y * ((float)inZ.getRowCount() / (float)im.height) + 20;
  
  //x = map(x, 0, im.width, inZ.getColumnCount(), 0);
  //y = map(y, 0, im.height, inZ.getRowCount(), 0);
   //x = x * ((float)inZ.getColumnCount() / (float)im.width);
  //y = y * ((float)inZ.getRowCount() / (float)im.height);
  
  x = floor(x);
  y = floor(y);
  
  
  while ((int) inZ.getFloat((int) x, (int) y) == 0){
    if (x > inZ.getColumnCount() / 2) x--;
    else x++;
    y--;
  }
  
  
  
  return inZ.getFloat((int)y, (int)x);
}
//1290, 869
//400, 129



void controlEvent(ControlEvent theEvent) {// event handlers for the control buttons
  if(theEvent.isController()) { 
    if(theEvent.getController().getName()=="x") {
      xdiv = floor(theEvent.getController().getValue());
    }
    if(theEvent.getController().getName()=="y") {
      ydiv = floor(theEvent.getController().getValue());
    }
  }
}





void mapZ(){
  for (int r = 2; r < out.getRowCount(); r += 3) {
    for (int c = 0; c < out.getColumnCount(); c++) {
      if (out.getFloat(r, c) > maxZ) {
        maxZ = out.getFloat(r, c);
      }
      if (out.getFloat(r, c) < minZ) {
        minZ = out.getFloat(r, c);
      }
    }
  }
  
  println(minZ, maxZ);
  
  
  
  
  for (int r = 2; r < out.getRowCount(); r += 3) {
    for (int c = 0; c < out.getColumnCount(); c++) {
      out.setFloat(r, c, map(out.getFloat(r, c), minZ, maxZ, 1.3, 0.3));
    }
  }
}

void draw(){
  frameRate(0);
  image(im, 0,0);
  tint(255, 128);
  image(a, 400, 129, (float)a.width / ((float)a.width / (float)xdiv), (float)a.height / ((float)a.height / (float)ydiv));
  noTint();
  stroke(255);
  csv();
}


/*
void draw(){
  image(a, 0, 0, a.width / 3, a.height / 3);
  image(im, 0,0);
  stroke(255);
  for (int r = 0; r < in.getRowCount(); r += 2) {
    for (int c = 0; c < in.getColumnCount(); c++) {
      point(xmap(in.getFloat(r + 1, c)), ymap(in.getFloat(r, c)) + 13);
    }
  }
}

float xmap(float x){
  x = map(x, 0, a.width, 445, (1290 + 400) - 55);
  //x = map(x, 400, (1290 + 400), 0, z.getColumnCount());
  
  if(x < z.getRowCount() / 2) x = ceil(x);
  else x = floor(x);

  return x;
}

float ymap(float y){
  y = map(y, 0, a.height, 129, (869 + 129));
  //y = map(y, 129, (869 + 129), 0, z.getRowCount());
  
  y = ceil(y);
  
  return y;
}
*/
