class Point{
  float x, y;

  Point (float x, float y) {
    this.x = x;
    this.y = y;
  }

  String toString() {
    return x + ", " + y;
  }
  
  int compare(float a, float b){
    return parseInt(a - b);
  }

}
