# Deep Seal Codex Bootstrap Files

Copy the contents of this bundle into the repository root:

```text
G:\unity_workspace\deep-seal
```

Preserve the folder structure.

After copying, run:

```powershell
git status
.\tools\verify-project.ps1
git add .
git commit -m "chore: add Codex project instructions and verification scripts"
git push
```

If PowerShell blocks script execution, run the command with a process-level bypass:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\verify-project.ps1
```

Do not use `Set-ExecutionPolicy` globally unless you understand the security implications.


## Manual Application Workflow

This bootstrap now sets Codex to proposal-only by default. Codex should propose code and exact edits, while the project owner manually applies changes. Codex then reviews the applied code or verification logs before the next step. Direct file edits require an explicit task-level request from the owner.
