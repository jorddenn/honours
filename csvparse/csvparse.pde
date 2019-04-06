Table in, out;

String file = "image1a";

float maxX = 0, maxY = 0;
float minX = MAX_FLOAT, minY = MAX_FLOAT;

void setup() {
  in = loadTable(file + ".csv");
  out = new Table();
  
  for (int r = 0; r < in.getRowCount(); r +=2) {
    for (int c = 0; c < in.getColumnCount(); c++) {
      if(in.getFloat(r + 1, c) > maxX){
        maxX = in.getFloat(r + 1, c);
      }
      if(in.getFloat(r, c) > maxY){
        maxY = in.getFloat(r, c);
      }
      if(in.getFloat(r + 1, c) < minX){
        minX = in.getFloat(r + 1, c);
      }
      if(in.getFloat(r, c) < minY){
        minY = in.getFloat(r, c);
      }
    }
  }
  
  for (int i = 0; i < in.getColumnCount(); i++){
    out.addColumn();
  }
  
  int outRow = 0;
  
  for (int r = 0; r < in.getRowCount(); r += 2) {
    out.addRow();
    out.addRow();
    out.addRow();
    for (int c = 0; c < in.getColumnCount(); c++) {
      out.setFloat(outRow, c, map(in.getFloat(r + 1, c), minX, maxX, 1.75, -1.75));
      out.setFloat(outRow + 1, c, map(in.getFloat(r, c), minY, maxY, 1.1, -3));
      out.setFloat(outRow + 2, c, 1.2);
    }
    outRow += 3;
  }
  
  println(in.getColumnCount(), in.getRowCount());
  println(out.getColumnCount(), out.getRowCount());
  
  saveTable(out, file + "z.csv");
}
