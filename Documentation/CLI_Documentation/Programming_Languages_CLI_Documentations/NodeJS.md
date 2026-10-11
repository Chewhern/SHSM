# Template
```
import { exec } from 'node:child_process';
import { promisify } from 'node:util';

const execPromise = promisify(exec);

async function runCli() {
  try {
    const { stdout, stderr } = await execPromise('SHSM_CLI genauinfo --algorithm ED25519 --public_contact test@example.com --duration 6');
    console.log(`${stdout}`);
  } catch (error) {
    console.error(`Error: ${error.message}`);
  }
}

runCli();
```
