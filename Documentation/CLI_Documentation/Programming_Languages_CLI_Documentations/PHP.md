# Method 1
```
<?php
// Using the function
$output = shell_exec('SHSM_CLI genauinfo --algorithm ED25519 --public_contact test@example.com --duration 6');

echo "<pre>$output</pre>";
?>
```

# Method 2
```
<?php
$outputLines = [];
$status = 0;

//OR

// Execute the command
exec('SHSM_CLI genauinfo --algorithm ED25519 --public_contact test@example.com --duration 6', $outputLines, $status);

if ($status === 0) {
    echo "Command succeeded!";
} else {
    echo "Command failed with code: " . $status;
}

echo "<pre>$outputLines[0]</pre>";
?>
```
