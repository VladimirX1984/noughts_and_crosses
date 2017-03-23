/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package ncserver.utils;

import javax.swing.JOptionPane;

/**
 *
 * @author Vladimir
 */
public final class MessageBox {

    public static void show(String infoMessage, String titleBar) {
        JOptionPane.
            showMessageDialog(null, infoMessage == null ? titleBar : infoMessage, titleBar, JOptionPane.INFORMATION_MESSAGE);
    }

    public static void show(String infoMessage, String titleBar, int jOptionPane) {
        JOptionPane.
            showMessageDialog(null, infoMessage == null ? titleBar : infoMessage, titleBar, jOptionPane);
    }

    public static void showError(String infoMessage, String titleBar) {
        JOptionPane.
            showMessageDialog(null, infoMessage == null ? titleBar : infoMessage, titleBar, JOptionPane.ERROR_MESSAGE);
    }

    public static void showWarning(String infoMessage, String titleBar) {
        JOptionPane.
            showMessageDialog(null, infoMessage == null ? titleBar : infoMessage, titleBar, JOptionPane.WARNING_MESSAGE);
    }
}
