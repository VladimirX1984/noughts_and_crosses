/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.game;

import ncserver.NC_GameState;

/**
 *
 * @author Vladimir
 */
public class GameStateChecker {

    public GameStateChecker() {

    }

    public static final int NONE = 0;
    public static final int WIN_X = 1;
    public static final int WIN_0 = 2;
    public static final int MATCH_DRAWN = 3;

    protected boolean isFilled(NC_GameState gameState, int numberToWinNeed, char cellValue,
                               char freeCell) {
        int lineCount = (int)Math.sqrt(gameState.count());

        int numberToWin = 0;

        for (int k = 0; k < lineCount - numberToWinNeed + 1; ++k) {
            // по горизонтали                
            for (int i = 0; i < lineCount; ++i) {
                numberToWin = 0;
                for (int j = 0; j < lineCount; ++j) {
                    int cellNumber = lineCount * i + j + k;
                    if (gameState.getCellValue(cellNumber) == cellValue
                        || gameState.getCellValue(cellNumber) == freeCell) {
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

        for (int k = 0; k < lineCount - numberToWinNeed + 1; ++k) {
            // по вертикали                
            for (int i = 0; i < lineCount; ++i) {
                numberToWin = 0;
                for (int j = 0; j < lineCount; ++j) {
                    int cellNumber = lineCount * j + i + lineCount * k;
                    if (gameState.getCellValue(cellNumber) == cellValue
                        || gameState.getCellValue(cellNumber) == freeCell) {
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
            for (int j = 0; j < lineCount - numberToWinNeed + 1; ++j) {
                numberToWin = 0;
                // по диагонали (слева-направо, сверху-вниз)
                for (int k = 0; k < lineCount; ++k) {
                    int cellNumber = lineCount * i + j + lineCount * k + k;

                    if (!gameState.isCellValid(cellNumber)) {
                        break;
                    }

                    if (gameState.getCellValue(cellNumber) == cellValue
                        || gameState.getCellValue(cellNumber) == freeCell) {
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
                for (int k = 0; k < lineCount; ++k) {
                    int cellNumber = -lineCount * i + j + lineCount * (lineCount - k - 1) + k;

                    if (!gameState.isCellValid(cellNumber)) {
                        break;
                    }

                    if (gameState.getCellValue(cellNumber) == cellValue
                        || gameState.getCellValue(cellNumber) == freeCell) {
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

    protected boolean isAllCellNoFilled(NC_GameState gameState) {
        // по диагонали (слева-направо, сверху-вниз)
        for (int i = 0; i < gameState.count(); ++i) {
            if (gameState.getCellValue(i) != '?') {
                return false;
            }
        }
        return true;
    }

    public boolean isAnyCellFilled(NC_GameState gameState) {
        for (int i = 0; i < gameState.count(); ++i) {
            if (gameState.getCellValue(i) != '?') {
                return true;
            }
        }
        return false;
    }

    public int getWinner(NC_GameState gameState, int numberToWinNeed) {
        if (isAllCellNoFilled(gameState)) {
            return NONE;
        }
        boolean bNotEnded = isFilled(gameState, numberToWinNeed, 'X', '?')
            || isFilled(gameState, numberToWinNeed, '0', '-');
        if (!bNotEnded) {
            return MATCH_DRAWN;
        }
        boolean bX = isFilled(gameState, numberToWinNeed, 'X', 'X');
        if (bX) {
            return WIN_X;
        }
        boolean b0 = isFilled(gameState, numberToWinNeed, '0', '0');
        if (b0) {
            return WIN_0;
        }
        return NONE;
    }
}
