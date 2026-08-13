# Template/Example
```
package main

import (
	"fmt"
	"log"
	"os/exec"
)

func main() {
	//Use either one depending on your use case/situation..
	//cmd := exec.Command("SHSM_CLI", "version")
	cmd := exec.Command(
		"SHSM_CLI",
		"genauinfo",
		"--algorithm",
		"ED25519",
		"--public_contact",
		"test@example.com",
		"--duration",
		"6",
	)

	output, err := cmd.Output()
	if err != nil {
		log.Fatalf("Command execution failed: %s", err)
	}
	fmt.Println(string(output))
}
```
