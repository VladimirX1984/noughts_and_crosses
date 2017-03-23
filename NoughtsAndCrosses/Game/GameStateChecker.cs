using System;

namespace NoughtsAndCrosses.Game {
  public class GameStateChecker {
    public GameStateChecker() {

    }

    public const byte NONE = 0;
    public const byte WIN_X = 1;
    public const byte WIN_0 = 2;
    public const byte MATCH_DRAWN = 3;

    protected bool IsFilled(GameState gameState, int numberToWinNeed, char cellValue, char freeCell) {
      int lineCount = (int) Math.Sqrt(gameState.Count);

      int numberToWin = 0;

      for (int k = 0; k < lineCount - numberToWinNeed + 1; k++) {
        // по горизонтали
        for (int i = 0; i < lineCount; ++i) {
          numberToWin = 0;
          for (int j = 0; j < lineCount; j++) {
            int cellNumber = lineCount * i + j + k;
            if (gameState.GetCellValue(cellNumber) == cellValue
                || gameState.GetCellValue(cellNumber) == freeCell) {
              numberToWin++;
            }
            else {
              numberToWin = 0;
            }
            if (numberToWin == numberToWinNeed) {
              return true;
            }
          }
        }
      }

      numberToWin = 0;

      for (int k = 0; k < lineCount - numberToWinNeed + 1; k++) {
        // по вертикали
        for (int i = 0; i < lineCount; ++i) {
          numberToWin = 0;
          for (int j = 0; j < lineCount; j++) {
            int cellNumber = lineCount * j + i + lineCount * k;
            if (gameState.GetCellValue(cellNumber) == cellValue
                || gameState.GetCellValue(cellNumber) == freeCell) {
              numberToWin++;
            }
            else {
              numberToWin = 0;
            }
            if (numberToWin == numberToWinNeed) {
              return true;
            }
          }
        }
      }

      // по диагонали (слева-направо, сверху-вниз)
      for (int i = 0; i < lineCount - numberToWinNeed + 1; ++i) {
        for (int j = 0; j < lineCount - numberToWinNeed + 1; j++) {
          numberToWin = 0;
          // по диагонали (слева-направо, сверху-вниз)
          for (int k = 0; k < lineCount; k++) {
            int cellNumber = lineCount * i + j + lineCount * k + k;

            if (!gameState.IsCellValid(cellNumber)) {
              break;
            }

            if (gameState.GetCellValue(cellNumber) == cellValue
                || gameState.GetCellValue(cellNumber) == freeCell) {
              numberToWin++;
            }
            else {
              numberToWin = 0;
            }
            if (numberToWin == numberToWinNeed) {
              return true;
            }
          }
          numberToWin = 0;
          // по диагонали (слева-направо, cнизу-наверх)
          for (int k = 0; k < lineCount; k++) {
            int cellNumber = -lineCount * i + j + lineCount * (lineCount - k - 1) + k;

            if (!gameState.IsCellValid(cellNumber)) {
              break;
            }

            if (gameState.GetCellValue(cellNumber) == cellValue
                || gameState.GetCellValue(cellNumber) == freeCell) {
              numberToWin++;
            }
            else {
              numberToWin = 0;
            }
            if (numberToWin == numberToWinNeed) {
              return true;
            }
          }
        }
      }
      return false;
    }

    protected bool IsAllCellNoFilled(GameState gameState) {
      // по диагонали (слева-направо, сверху-вниз)
      for (int i = 0; i < gameState.Count; ++i) {
        if (gameState.GetCellValue(i) != '?') {
          return false;
        }
      }
      return true;
    }

    public bool IsAnyCellFilled(GameState gameState) {
      for (int i = 0; i < gameState.Count; ++i) {
        if (gameState.GetCellValue(i) != '?') {
          return true;
        }
      }
      return false;
    }

    public int GetWinner(GameState gameState, int numberToWinNeed) {
      if (IsAllCellNoFilled(gameState)) {
        return NONE;
      }
      bool bNotEnded = IsFilled(gameState, numberToWinNeed, 'X', '?') || IsFilled(gameState, numberToWinNeed, '0', '?');
      if (!bNotEnded) {
        return MATCH_DRAWN;
      }
      bool bX = IsFilled(gameState, numberToWinNeed, 'X', 'X');
      if (bX) {
        return WIN_X;
      }
      bool b0 = IsFilled(gameState, numberToWinNeed, '0', '0');
      if (b0) {
        return WIN_0;
      }
      return NONE;
    }
  }
}
