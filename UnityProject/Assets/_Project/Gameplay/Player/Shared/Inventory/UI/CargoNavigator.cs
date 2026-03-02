using UnityEngine;

public class CargoNavigator {
    private readonly int columns;
    private readonly int rows;
    private readonly int gridCount;
    private readonly int slotDropL;
    private readonly int slotDropR;
    private readonly int slotQuick1;
    private readonly int slotQuick2;
    private readonly int slotQuick3;
    private readonly int slotQuick4;
    private readonly int slotTrash;


    public CargoNavigator(int columns, int rows) {

        // SAFEGUARD - enforce minimum grid size
        if (columns < 4) {
            Debug.LogWarning($"CargoNavigator requires at least 4 columns. " + $"Received {columns}, clamped to 4.");
            this.columns = 4;
        } else {
            this.columns = columns;
        }
        if (rows < 1) {
            Debug.LogWarning($"CargoNavigator requires at least 1 row. " + $"Received {rows}, clamped to 1.");
            this.rows = 1;
        } else {
            this.rows = rows;
        }
        
        gridCount = this.columns * this.rows;
        slotDropL = gridCount + 0;
        slotDropR = gridCount + 1;
        slotQuick1 = gridCount + 2;
        slotQuick2 = gridCount + 3;
        slotQuick3 = gridCount + 4;
        slotQuick4 = gridCount + 5;
        slotTrash  = gridCount + 6;
    }
    public int GetNextIndex(int current, Vector2 dir) {
        if (dir.x < 0)  return MoveLeft(current);
        if (dir.x > 0)  return MoveRight(current);
        if (dir.y > 0)  return MoveUp(current);
        if (dir.y < 0)  return MoveDown(current);
        return current;
    }

    // LEFT
    private int MoveLeft(int current) {
        if (IsFirstColumn(current) || current == slotQuick1) return slotDropL;      //Left edge -> DL
        if (current == slotDropL) return slotDropR;                                 //DL -> DR
        if (current == slotDropR) return gridCount-1;                               //DR -> gridEnd.    (CHECK)
        if (current == slotTrash) return slotQuick4;                                //T -> Q4
        if (current == slotQuick4) return slotQuick3;
        if (current == slotQuick3) return slotQuick2;
        if (current == slotQuick2) return slotQuick1;
        return current - 1;
    }
    // RIGHT
    private int MoveRight(int current) {
        if (IsLastColumn(current) || current == slotTrash) return slotDropR;        //Right edge -> DR
        if (current == slotDropR) return slotDropL;                                 //DR -> DL
        if (current == slotDropL) return 0;                                         //DL -> gridStart   (CHECK)
        if (current == slotQuick4) return slotTrash;                                //Q4 -> T
        if (current == slotQuick3) return slotQuick4;
        if (current == slotQuick2) return slotQuick3;
        if (current == slotQuick1) return slotQuick2;
        return current + 1;
    }
    // UP
    private int MoveUp(int current) {
        if (current == slotDropL || current == slotDropR) return current;                      //does nothing
        if (IsFirstRow(current)) {
            int index = current % columns;
            return index switch {
                0 => slotQuick1,
                1 => slotQuick2,
                2 => slotQuick3,
                3 => slotQuick4,
                _ => slotTrash    
            };
        }
        int lastRowStart = (rows-1) * columns;
        if (current == slotQuick1) return lastRowStart + 0;                         
        if (current == slotQuick2) return lastRowStart + 1;
        if (current == slotQuick3) return lastRowStart + 2;
        if (current == slotQuick4) return lastRowStart + 3;
        if (current == slotTrash) return gridCount-1;
        return current - columns;        
    }
    // DOWN
    private int MoveDown(int current) {
        if (current == slotDropL || current == slotDropR) return current;                      //does nothing
        if (IsLastRow(current)) {
            int index = current % columns;
            return index switch {
                0 => slotQuick1,
                1 => slotQuick2,
                2 => slotQuick3,
                3 => slotQuick4,
                _ => slotTrash                       
            };
        }
        if (current == slotQuick1) return 0;                         
        if (current == slotQuick2) return 1;                                        //quickslots to top row
        if (current == slotQuick3) return 2;                                        //quickslots to top row
        if (current == slotQuick4) return 3;                                        //quickslots to top row
        if (current == slotTrash) return columns -1;                                //last index in first row
        return current + columns;                                                   //normal downward grid movement
    }


    // -------- HELPERS ----------
    private bool IsFirstColumn(int index) {
        return (index >= 0) && (index < gridCount) && ((index % columns) == 0);
    }
    private bool IsLastColumn(int index) {
        return (index >= 0) && (index < gridCount) && ((index % columns) == columns - 1);
    }
    private bool IsFirstRow(int index) {
        return (index >= 0) && (index < gridCount) && (index < columns);
    }
    private bool IsLastRow(int index) {
        return (index >= 0) && (index < gridCount) && (index >= (rows - 1) * columns);
    }
}
