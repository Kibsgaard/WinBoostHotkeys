# AI Development Guidelines for WinBoostHotkeys

## General Principles

### Response Style
- **Coding tasks**: Keep responses concise, focus on implementation, minimal summaries
- **Planning/discussion**: Full explanations and suggestions are welcome
- **Questions**: Provide clear, direct answers

### Cost Efficiency
- **Batch tool calls**: When possible, read multiple files or perform multiple searches in parallel
- **Avoid redundant operations**: Don't re-read files unnecessarily, check context first
- **Focused searches**: Use specific queries rather than broad exploratory searches
- **Incremental changes**: Make small, focused edits rather than large rewrites

### Code Quality
- **Follow existing patterns**: Maintain consistency with codebase style
- **Error handling**: Include proper error handling and edge cases
- **Comments**: Add comments for complex logic, but keep code self-documenting
- **No over-engineering**: Keep solutions simple and practical for a hobby project

## Project-Specific Guidelines

### File Organization
- Follow the structure defined in `plan.md`
- Keep classes focused and single-purpose
- Use clear, descriptive names

### Localization
- All user-facing strings must use resource files (Strings.resx)
- Never hardcode UI text directly in code
- Use resource keys consistently

### Settings
- Settings are stored in JSON format in AppData
- Always validate settings on load
- Provide sensible defaults

### Power Plan Operations
- Always handle both AC and DC power states (`/setacvalueindex` and `/setdcvalueindex`)
- Verify operations succeeded when possible
- Handle cases where power plan operations might fail (permissions, etc.)

### Error Handling
- Log errors appropriately (consider simple file logging or Windows Event Log)
- Don't crash on non-critical errors (e.g., network detection failures)
- Provide user feedback for critical failures

## Communication Patterns

### When Requesting Implementation
**Good prompt:**
```
"Implement PowerPlanManager class with GetCurrentBoostMode() and SetBoostMode() methods"
```

**Less efficient:**
```
"Can you help me with power plan management? I need to get and set boost mode. 
Also, what do you think about the architecture? Should we use powercfg or APIs? 
And how should we handle errors? Maybe we should add logging too..."
```

### When Asking Questions
**Good prompt:**
```
"How should we handle hotkey conflicts if the user's chosen hotkey is already registered?"
```

**Good prompt:**
```
"What's the best approach for detecting WiFi SSID changes in .NET?"
```

### When Reviewing Code
**Good prompt:**
```
"Review PowerPlanManager.cs for potential issues"
```

## Implementation Workflow

1. **Read relevant files first** - Understand existing code before making changes
2. **Make focused changes** - One class/feature at a time when possible
3. **Test incrementally** - Verify each component works before moving on
4. **Update plan** - Mark completed tasks in plan.md if helpful

## Tool Usage Best Practices

### File Operations
- Read files before editing (unless creating new)
- Use `search_replace` for precise edits
- Use `write` only for new files
- Batch file reads when examining multiple related files

### Code Search
- Use `codebase_search` for semantic understanding ("How does X work?")
- Use `grep` for exact string/symbol searches
- Prefer targeted searches over broad exploration

### Terminal Commands
- Only run commands when necessary (build, test, etc.)
- Don't run commands that require user interaction
- Use background processes for long-running tasks

## Cost-Saving Tips

1. **Batch operations**: Read multiple files in parallel
2. **Focused queries**: Be specific in searches
3. **Incremental work**: Don't ask AI to read entire codebase unless necessary
4. **Reuse context**: Reference previous conversations rather than re-exploring
5. **Skip unnecessary steps**: Don't lint/test unless you've made changes

## Example Interaction Patterns

### Efficient Coding Request
```
User: "Add error handling to PowerPlanManager.SetBoostMode()"
AI: [Makes focused edit, minimal explanation]
```

### Discussion Request
```
User: "Should we use async/await for power plan operations?"
AI: [Provides analysis, pros/cons, recommendation]
```

### Multi-step Task
```
User: "Implement the Settings UI form"
AI: [Creates SettingsForm.cs, updates related files, concise summary]
```

## Notes for First-Time AI Coding

- **Be specific**: Clear requests get better results
- **Iterate**: Start simple, refine as needed
- **Review code**: Always review AI-generated code before committing
- **Ask questions**: If something is unclear, ask for clarification
- **Break it down**: Large tasks work better when broken into smaller steps
- **Use version control**: Commit frequently to track changes

## Project Status Tracking

- Check `plan.md` for current phase and priorities
- Mark completed items
- Focus on one phase at a time
