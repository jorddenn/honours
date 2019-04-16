
/*
1a 35




*/


import controlP5.*;

Path[] paths;

Table table;

String file = "image1";

int divisor = 3;
int percentStrands = 100;

PImage img;

ControlP5 p5;

void setup() {
  size(1, 1);
  surface.setResizable(true);

  img = loadImage(file + ".jpg");

  surface.setSize(img.width / divisor, img.height / divisor);
  
  table = loadTable(file + ".csv");
  //table = loadTable("imageall.csv");
  paths = new Path[table.getRowCount() / 2];
  for (int r = 0; r < table.getRowCount(); r +=2) {
    paths[r / 2] = new Path(table.getColumnCount());
    for (int c = 0; c < table.getColumnCount(); c++) {
      paths[r / 2].set(table.getFloat(r + 1, c), table.getFloat(r, c), c);
    }
    paths[r / 2].tall();
  }

  java.util.Arrays.sort(paths);
  for (int b = 0; b < paths.length; b++) {
    println(paths[b].tall);
  }
  
  p5 = new ControlP5(this);
  p5.addSlider("Percent Strands", 0, 100, 100, 0, 100, 200, 25);
}

void controlEvent(ControlEvent theEvent) {// event handlers for the control buttons
  if(theEvent.isController()) { 
    if(theEvent.getController().getName()=="Percent Strands") {
      percentStrands = floor(theEvent.getController().getValue());
    }
  }
}

void overlay() {
  //frameRate(0);

  stroke(255, 0, 255);
  strokeWeight(1);

  for (int r = 0; r < paths.length; r++) {
    for (int c = 0; c < paths[r].getLength() - 1; c++) {
      line(paths[r].path[c].x / divisor, paths[r].path[c].y / divisor, paths[r].path[c + 1].x / divisor, paths[r].path[c + 1].y / divisor);
    }
  }
}

void limitedOverlay() {

  for (int r = 0; r < paths.length; r++) {
    if (paths[r].tall > paths[paths.length - 1].tall * percentStrands / 100) {
      for (int c = 0; c < paths[r].getLength() - 1; c++) {
        line(paths[r].path[c].x / divisor, paths[r].path[c].y / divisor, paths[r].path[c + 1].x / divisor, paths[r].path[c + 1].y / divisor);
      }
    }
  }
}

void strands() {

  for (int r = 0; r < paths.length; r++) {
    if (paths[r].tall > paths[paths.length - 1].tall * percentStrands / 100) stroke(0, 255, 0);
    for (int c = 0; c < paths[r].getLength() - 1; c++) {
      line(r + (width - paths.length), 10, r + (width - paths.length), paths[r].tall / divisor);
    }
  }
}

void draw() {
  background(0);
  image(img, 0, 0, img.width/ divisor, img.height / divisor);
  stroke(255, 0, 255);
  strokeWeight(1);
  //overlay();
  limitedOverlay();
  strands();
}
