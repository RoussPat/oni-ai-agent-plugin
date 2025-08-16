# Workers Management System

## Overview

This document defines the communication protocol and task management system between the **Manager Agent** (me) and **Worker Agents** (you) for the ONI AI Agent Plugin project. The manager coordinates development tasks while workers execute specific assignments.

## Communication Protocol

### Task Assignment Format

When I assign a task to a worker, I will use this format:

```
**TASK ASSIGNMENT**
Worker: [Worker Name]
Task ID: [Unique Identifier]
Priority: [High/Medium/Low]
Estimated Time: [Time Estimate]

**DESCRIPTION**
[Detailed task description]

**REQUIREMENTS**
- [Requirement 1]
- [Requirement 2]
- [Requirement 3]

**ACCEPTANCE CRITERIA**
- [Criterion 1]
- [Criterion 2]
- [Criterion 3]

**RESOURCES**
- Files: [List of relevant files]
- Dependencies: [Any dependencies]
- References: [Links or documentation]

**COMMANDS TO EXECUTE**
```bash
[Specific commands to run]
```

**REPORTING FORMAT**
[How to report back results]
```

### Worker Response Format

Workers must respond using this format:

```
**WORKER RESPONSE**
Worker: [Worker Name]
Task ID: [Task ID]
Status: [In Progress/Completed/Failed/Blocked]

**PROGRESS UPDATE**
[Current status and progress]

**COMMANDS EXECUTED**
```bash
[Commands that were run]
```

**OUTPUT/RESULTS**
[Any output, errors, or results]

**NEXT STEPS**
[What needs to be done next]

**BLOCKERS/ISSUES**
[Any problems encountered]

**FILES MODIFIED**
[List of files changed, if any]
```

## Worker Roles

### 1. **Build Worker**
- **Responsibilities**: Building, compiling, and packaging the plugin
- **Skills**: .NET, MSBuild, build scripts
- **Tasks**: Compilation, DLL generation, build verification

### 2. **Test Worker**
- **Responsibilities**: Testing and quality assurance
- **Skills**: Unit testing, integration testing, game testing
- **Tasks**: Test execution, bug reporting, test coverage

### 3. **Code Worker**
- **Responsibilities**: Code development and implementation
- **Skills**: C#, Unity, ONI modding, AI/ML
- **Tasks**: Feature implementation, bug fixes, code refactoring

### 4. **Documentation Worker**
- **Responsibilities**: Documentation and guides
- **Skills**: Technical writing, markdown, user guides
- **Tasks**: README updates, API documentation, user guides

### 5. **UI Worker**
- **Responsibilities**: User interface development
- **Skills**: Unity UI, C# UI programming, UX design
- **Tasks**: UI components, dialogs, user experience

### 6. **Integration Worker**
- **Responsibilities**: System integration and compatibility
- **Skills**: Harmony patching, game integration, plugin systems
- **Tasks**: Game integration, compatibility testing, plugin loading

## Task Categories

### **High Priority Tasks**
- Critical bug fixes
- Build failures
- Game compatibility issues
- Security vulnerabilities

### **Medium Priority Tasks**
- Feature development
- Performance improvements
- Code refactoring
- Documentation updates

### **Low Priority Tasks**
- Code cleanup
- Minor UI improvements
- Additional features
- Optimization

## Command Templates

### Build Commands
```bash
# Windows Build
scripts\build.bat

# Linux/Mac Build
./scripts/build.sh

# Manual Build
dotnet build --configuration Release

# Clean Build
dotnet clean && dotnet build --configuration Release
```

### Test Commands
```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter "TestName"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Code Quality Commands
```bash
# Format code
dotnet format

# Analyze code
dotnet analyze

# Check for security issues
dotnet list package --vulnerable
```

## File Structure Guidelines

### Source Code Organization
```
src/
├── Core/           # Core plugin functionality
├── Agents/         # AI agent implementations
├── UI/            # User interface components
├── Patches/       # Harmony patches
└── Config/        # Configuration management
```

### Documentation Organization
```
docs/
├── README.md              # Main documentation
├── WORKERS.md            # This file
├── AGENT_README.md       # Agent development guide
├── TROUBLESHOOTING.md    # Common issues and solutions
└── [SPECIFIC_GUIDES].md  # Task-specific guides
```

## Quality Standards

### Code Standards
- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods under 50 lines when possible
- Use proper error handling

### Documentation Standards
- Use clear, concise language
- Include code examples
- Provide step-by-step instructions
- Update related documentation when making changes

### Testing Standards
- Write unit tests for new features
- Test edge cases and error conditions
- Verify integration with ONI game
- Test on multiple platforms when possible

## Error Handling Protocol

### When Tasks Fail
1. **Immediate Response**: Report failure within 5 minutes
2. **Error Details**: Provide complete error messages and stack traces
3. **Context**: Explain what was being attempted
4. **Suggestions**: Propose potential solutions
5. **Escalation**: Request manager assistance if needed

### Common Error Categories
- **Build Errors**: Compilation failures, missing dependencies
- **Runtime Errors**: Game crashes, plugin loading issues
- **Integration Errors**: Harmony patch failures, game compatibility
- **Performance Issues**: Memory leaks, slow execution

## Reporting Guidelines

### Daily Status Reports
Workers should provide daily status updates:
- Tasks completed
- Tasks in progress
- Blockers encountered
- Time estimates for remaining work

### Weekly Reviews
Manager will conduct weekly reviews:
- Progress assessment
- Priority adjustments
- Resource allocation
- Process improvements

## Communication Channels

### Primary Communication
- **Task Assignment**: Through this system
- **Status Updates**: Using the defined format
- **Questions**: Direct to manager with context

### Escalation Path
1. **Worker Level**: Attempt to resolve independently
2. **Manager Level**: Request assistance with details
3. **Team Level**: Escalate complex issues

## Success Metrics

### Individual Worker Metrics
- Task completion rate
- Code quality scores
- Test coverage
- Documentation completeness

### Team Metrics
- Project velocity
- Bug resolution time
- Feature delivery time
- User satisfaction

## Emergency Procedures

### Critical Issues
1. **Immediate Notification**: Alert manager immediately
2. **Impact Assessment**: Evaluate scope and severity
3. **Mitigation Plan**: Propose immediate actions
4. **Recovery Steps**: Plan for resolution

### Rollback Procedures
- Keep previous working versions
- Document changes for rollback
- Test rollback procedures
- Maintain backup strategies

---

## Quick Reference

### Task Assignment Template
```
**TASK ASSIGNMENT**
Worker: [Name]
Task ID: [ID]
Priority: [Level]
Estimated Time: [Time]

**DESCRIPTION**
[Task details]

**REQUIREMENTS**
- [Requirements]

**ACCEPTANCE CRITERIA**
- [Criteria]

**COMMANDS TO EXECUTE**
```bash
[Commands]
```

**REPORTING FORMAT**
[Format]
```

### Worker Response Template
```
**WORKER RESPONSE**
Worker: [Name]
Task ID: [ID]
Status: [Status]

**PROGRESS UPDATE**
[Progress]

**COMMANDS EXECUTED**
```bash
[Commands]
```

**OUTPUT/RESULTS**
[Results]

**NEXT STEPS**
[Next steps]

**BLOCKERS/ISSUES**
[Issues]

**FILES MODIFIED**
[Files]
```

---

*This document is maintained by the Manager Agent and should be updated as the project evolves.*
