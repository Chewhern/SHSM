# Template
```
/*
 * Click nbfs://nbhost/SystemFileSystem/Templates/Licenses/license-default.txt to change this license
 * Click nbfs://nbhost/SystemFileSystem/Templates/Classes/Class.java to edit this template
 */
package TestJava;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.IOException;

public class Test2Class {
    
    public static void main(String[] args)
    {
        //Use either one..
        //String[] command = {"SHSM_CLI", "version"}; 
        String[] command = {"SHSM_CLI", "genauinfo", "--algorithm", "ED25519", "--public_contact", "test@example.com", "--duration", "6"}; 
        
        // Initialize the ProcessBuilder
        ProcessBuilder processBuilder = new ProcessBuilder(command);
        
        try {
            // Start the external CLI application
            Process process = processBuilder.start();
            
            // Read the output from the CLI command
            try (BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream()))) {
                String line;
                while ((line = reader.readLine()) != null) {
                    System.out.println(line);
                }
            }
            
            // Wait for the process to complete and get the exit code
            int exitCode = process.waitFor();
            System.out.println("\nProcess exited with code: " + exitCode);
            
        } catch (IOException | InterruptedException e) {
            e.printStackTrace();
        }
    }
}
```
