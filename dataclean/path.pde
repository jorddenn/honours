class Path implements Comparable<Path>{
  Point[] path;
  float tall;

  Path(int len) {
    super();
    path = new Point[len];
    for (int i = 0; i < path.length; i++) {
      path[i] = new Point( -1, -1);
    }
  }

  void set(float x, float y, int index) {
    path[index].x = x;
    path[index].y = y;
  }
  
  void tall(){
    if (path[path.length - 1].y - path[0].y < 0){
      for(int r = 0; r < path.length / 2; r++){
        float temp = path[r].y;
        path[r].y = path[path.length - r].y;
        path[path.length - r].y = temp;
      }
    }
    this.tall = path[path.length - 1].y - path[0].y;
  }
  
  int getLength(){
    return path.length;
  }
  
  int compareTo(Path path){
    return parseInt(this.tall - path.tall);
  }
}
