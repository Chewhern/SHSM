# Template
```
import subprocess

result = subprocess.run(
    [
        "SHSM_CLI",
        "genauinfo",
        "--algorithm",
        "ED25519",
        "--public_contact",
        "test@example.com",
        "--duration",
        "6"
    ],
    capture_output=True,
    text=True
)

if result.returncode != 0:
    print("Command execution failed:")
    print(result.stderr)
else:
    print(result.stdout)
```
