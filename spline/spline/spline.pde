/*
TODO
flip negative height lines
order with constant space on the screen
look into joining small lines









*/












PImage img;
Point[][] pnt;
float tall[];
int current = 0;
Table table;
int divisor = 3;

String image = "image2";

void setup() {
  img = loadImage(image + ".jpg");

  size (200, 200);
  surface.setResizable(true);
  surface.setSize(img.width / divisor, img.height / divisor);

  table = loadTable(image + ".csv");

  pnt = new Point[table.getRowCount() / 2][table.getColumnCount()];
  tall = new float[table.getRowCount() / 2];

  for (int r = 0; r < table.getRowCount()/ 2; r ++) {
    for (int c = 0; c < table.getColumnCount(); c ++) {
      pnt[r][c] = new Point(table.getFloat(r, c) / divisor, table.getFloat(r + 1, c) / divisor);
    }
  }

  java.util.Arrays.sort(pnt, new java.util.Comparator<Point[]>() {
    public int compare(Point[] a, Point[] b) {
      return Float.compare(abs(a[19].y - a[0].y), abs(b[19].y - b[0].y));
    }
  }
  );
  
  for (int v = 0 ; v < table.getRowCount()/ 2; v+= 2){
    print(pnt[v][19].y - pnt[v][0].y + " ");
  }
  
  /*
  
  for (int v = 0 ; v < table.getRowCount()/ 2; v++){
    float[] tall = new float[19];
    for(int g = 0; g < 19; g++){
      tall[g] = pnt[v]
    }
  }
  
  */
}

void overlay() {
  image(img, 0, 0, img.width / divisor, img.height / divisor);
  frameRate(0);

  stroke(255, 0, 255);
  strokeWeight(2);

  //current = (current + 2) % (table.getRowCount() / 2);

  for (current = 0; current < table.getRowCount() / 2; current++) {
    for (int i = 0; i < 19; i++) {
      line(pnt[current][i].y, pnt[current][i].x, pnt[current][i + 1].y, pnt[current][i + 1].x);
    }
  }
}

void onebyone() {
  image(img, 0, 0, img.width / divisor, img.height / divisor);
  frameRate(10);

  stroke(255, 0, 255);
  strokeWeight(2);

  current = (current + 2) % (table.getRowCount() / 2);

  for (int i = 0; i < 19; i++) {
    line(pnt[current][i].y, pnt[current][i].x, pnt[current][i + 1].y, pnt[current][i + 1].x);
  }
}

void small2big() {
  image(img, 0, 0, img.width / divisor, img.height / divisor);
  frameRate(0);

  stroke(255, 0, 255);
  strokeWeight(2);

  for (current = 0; current < table.getRowCount() / 2; current += 2) {
    for (int i = 0; i < 19; i++) {
      line(current, pnt[current][i].y, current, pnt[current][i + 1].y);
    }
  }
}


void draw() {
  background(255);
  //onebyone();
  overlay();
}
