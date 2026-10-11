# Modern C#
```
var processInfo = new ProcessStartInfo
{
    FileName = "SHSM_CLI",
    Arguments = "version",

    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    CreateNoWindow = true
};

using var process = new Process
{
    StartInfo = processInfo
};

process.OutputDataReceived += (sender, e) =>
{
    if (e.Data != null)
    {
        MessageBox.Show($"{e.Data}");
    }
};

process.ErrorDataReceived += (sender, e) =>
{
    if (e.Data != null)
    {
        MessageBox.Show($"CLI ERROR: {e.Data}");
    }
};

process.Start();

process.BeginOutputReadLine();
process.BeginErrorReadLine();

await process.WaitForExitAsync();

MessageBox.Show($"Exit code: {process.ExitCode}");
```

This code is primarily using newer or modern version of .NET.

I personally test it with .NET 8 (Winforms).

This could be served as a template.

In most cases, modern version of .NET may not require the use of **SHSM_CLI**

# Older/deprecated C#
```
var processInfo = new ProcessStartInfo
{
    FileName = "SHSM_CLI",
    Arguments = "version",

    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    CreateNoWindow = true
};

using var process = new Process
{
    StartInfo = processInfo
};

process.OutputDataReceived += (sender, e) =>
{
    if (e.Data != null)
    {
        MessageBox.Show($"{e.Data}");
    }
};

process.ErrorDataReceived += (sender, e) =>
{
    if (e.Data != null)
    {
        MessageBox.Show($"CLI ERROR: {e.Data}");
    }
};

process.Start();

process.BeginOutputReadLine();
process.BeginErrorReadLine();

process.WaitForExit();

MessageBox.Show($"Exit code: {process.ExitCode}");
```

## Purpose
This template serves as an introduction or tutorial for developers to get data from CLI as required.

The exact code may be modified depending on your own project's requirements.
