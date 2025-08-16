#!/bin/bash

# AI Agent Plugin Management Script
# Combines building, testing, installation verification, and Ollama integration

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}✓${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

print_info() {
    echo -e "${BLUE}ℹ${NC} $1"
}

# Function to show usage
show_usage() {
    echo "AI Agent Plugin Management Script"
    echo ""
    echo "Usage: $0 [COMMAND]"
    echo ""
    echo "Commands:"
    echo "  build          Build the mod (default)"
    echo "  test           Test installation and files"
    echo "  ollama         Test Ollama integration"
    echo "  install        Install the mod to ONI"
    echo "  logs           Check game logs for mod messages"
    echo "  full           Build, test, and check Ollama (recommended)"
    echo "  clean          Clean build artifacts"
    echo "  help           Show this help"
    echo ""
    echo "Examples:"
    echo "  $0              # Build the mod"
    echo "  $0 full         # Complete build and test"
    echo "  $0 install      # Install to ONI"
    echo "  $0 logs         # Check if mod is loading properly"
}

# Function to build the mod
build_mod() {
    echo "=== Building AI Agent Plugin ==="
    echo ""
    
    export PATH="$PATH:/home/pat/.dotnet"
    
    if dotnet build --configuration Release; then
        print_status "Build successful!"
        echo ""
        print_info "Build artifacts:"
        echo "  - bin/Release/net471/AIAgentPlugin.dll"
        echo "  - mod_info.yaml"
        echo "  - mod.yaml"
    else
        print_error "Build failed!"
        exit 1
    fi
}

# Function to test installation and files
test_installation() {
    echo "=== Testing Installation ==="
    echo ""
    
    local all_good=true
    
    # Check if build was successful
    if [ -f "bin/Release/net471/AIAgentPlugin.dll" ]; then
        print_status "DLL file exists: bin/Release/net471/AIAgentPlugin.dll"
    else
        print_error "DLL file missing: bin/Release/net471/AIAgentPlugin.dll"
        print_warning "Run '$0 build' first"
        all_good=false
    fi
    
    # Check required mod files
    if [ -f "mod_info.yaml" ]; then
        print_status "mod_info.yaml exists"
    else
        print_error "mod_info.yaml missing"
        all_good=false
    fi
    
    if [ -f "mod.yaml" ]; then
        print_status "mod.yaml exists"
    else
        print_error "mod.yaml missing"
        all_good=false
    fi
    
    # Check source files
    echo ""
    echo "=== Source Files ==="
    source_files=(
        "Source/AIAgent.cs"
        "Source/HelloWorldAgent.cs"
        "Source/GPTAgent.cs"
        "Source/AgentManager.cs"
        "Source/AgentControlDialog.cs"
        "Source/AIAgentPlugin.cs"
        "Source/Mod.cs"
        "Source/FloatingUIManager.cs"
        "Source/SimpleLogsDialog.cs"
    )
    
    for file in "${source_files[@]}"; do
        if [ -f "$file" ]; then
            print_status "$file"
        else
            print_error "$file"
            all_good=false
        fi
    done
    
    # Check documentation
    echo ""
    echo "=== Documentation ==="
    if [ -f "AGENT_README.md" ]; then
        print_status "AGENT_README.md exists"
    else
        print_error "AGENT_README.md missing"
        all_good=false
    fi
    
    if [ -f "README.md" ]; then
        print_status "README.md exists"
    else
        print_error "README.md missing"
        all_good=false
    fi
    
    if [ "$all_good" = true ]; then
        echo ""
        print_status "All files present and ready!"
    else
        echo ""
        print_error "Some files are missing. Please check the errors above."
        return 1
    fi
}

# Function to test Ollama integration
test_ollama() {
    echo "=== Testing Ollama Integration ==="
    echo ""
    
    # Check if Ollama is running
    echo "1. Checking Ollama service..."
    if curl -s http://localhost:11434/api/tags > /dev/null; then
        print_status "Ollama service is running on http://localhost:11434"
    else
        print_warning "Ollama service is not running"
        echo "   Starting Ollama service..."
        ollama serve &
        sleep 3
        if curl -s http://localhost:11434/api/tags > /dev/null; then
            print_status "Ollama service started successfully"
        else
            print_error "Failed to start Ollama service"
            return 1
        fi
    fi
    
    # Check if GPT-OSS-20B model is available
    echo ""
    echo "2. Checking GPT-OSS-20B model..."
    if ollama list | grep -q "gpt-oss:20b"; then
        print_status "GPT-OSS-20B model is available"
    else
        print_error "GPT-OSS-20B model not found"
        echo "   Available models:"
        ollama list
        return 1
    fi
    
    # Test the model with a simple prompt
    echo ""
    echo "3. Testing GPT-OSS-20B model..."
    echo "Sending test prompt to the model..."
    
    curl -s -X POST http://localhost:11434/api/generate \
      -d '{
        "model": "gpt-oss:20b",
        "prompt": "You are an AI assistant helping to manage an Oxygen Not Included colony. Please provide a brief tip for new players.",
        "stream": false,
        "options": {
          "temperature": 0.7,
          "num_predict": 100
        }
      }' > /tmp/ollama_test_response.json
    
    if [ -s /tmp/ollama_test_response.json ]; then
        print_status "Model test successful!"
        echo "Response received (check /tmp/ollama_test_response.json for details)"
        echo "Sample response:"
        head -c 200 /tmp/ollama_test_response.json
        echo "..."
    else
        print_error "Model test failed"
        return 1
    fi
    
    echo ""
    echo "=== Ollama Integration Ready ==="
    print_status "GPT-OSS-20B integration is working!"
    echo ""
    print_info "API Configuration:"
    echo "  - URL: http://localhost:11434"
    echo "  - Model: gpt-oss:20b"
    echo "  - Endpoint: /api/generate"
}

# Function to install the mod
install_mod() {
    echo "=== Installing AI Agent Plugin ==="
    echo ""
    
    # Check if build exists
    if [ ! -f "bin/Release/net471/AIAgentPlugin.dll" ]; then
        print_error "Build not found. Run '$0 build' first."
        return 1
    fi
    
    # Determine mod directory
    local mod_dir=""
    if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "cygwin" ]]; then
        # Windows
        mod_dir="$USERPROFILE/Documents/Klei/OxygenNotIncluded/mods/Dev/AIAgentPlugin"
    else
        # Linux/Mac
        mod_dir="$HOME/.config/unity3d/Klei/Oxygen Not Included/mods/Dev/AIAgentPlugin"
    fi
    
    # Create mod directory
    echo "Creating mod directory: $mod_dir"
    mkdir -p "$mod_dir"
    
    # Copy files
    echo "Copying mod files..."
    cp "bin/Release/net471/AIAgentPlugin.dll" "$mod_dir/"
    cp "mod_info.yaml" "$mod_dir/"
    cp "mod.yaml" "$mod_dir/"
    
    print_status "Mod installed successfully!"
    echo ""
    print_info "Installation location: $mod_dir"
    echo ""
    print_info "Next steps:"
    echo "1. Start Oxygen Not Included"
    echo "2. Enable the 'AI Agent Plugin' mod"
    echo "3. Start a new game or load existing save"
    echo "4. Look for the green 'AI' button in the game toolbar"
    echo "5. Run '$0 logs' to check if mod is loading properly"
}

# Function to check game logs
check_logs() {
    echo "=== AI Agent Plugin Log Checker ==="
    echo ""
    
    # Check multiple possible log locations (Windows paths through WSL)
    LOG_DIRS=(
        "/mnt/c/Users/patri/AppData/LocalLow/Klei/Oxygen Not Included"
        "/mnt/c/Users/patri/AppData/Local/Klei/Oxygen Not Included"
        "/mnt/c/Users/patri/Documents/Klei/OxygenNotIncluded"
        "/mnt/c/Users/$USER/AppData/LocalLow/Klei/Oxygen Not Included"
        "/mnt/c/Users/$USER/AppData/Local/Klei/Oxygen Not Included"
        "/mnt/c/Users/$USER/Documents/Klei/OxygenNotIncluded"
        "$HOME/.config/unity3d/Klei/Oxygen Not Included"
    )
    
    LOG_DIR=""
    for dir in "${LOG_DIRS[@]}"; do
        if [ -d "$dir" ]; then
            LOG_DIR="$dir"
            print_info "Found log directory: $LOG_DIR"
            break
        fi
    done
    
    if [ -z "$LOG_DIR" ]; then
        print_error "No log directory found. Checked locations:"
        for dir in "${LOG_DIRS[@]}"; do
            echo "  - $dir"
        done
        echo ""
        echo "Please make sure Oxygen Not Included is installed and has been run at least once."
        return 1
    fi
    
    print_info "Using log directory: $LOG_DIR"
    echo ""
    
    # Find the most recent log file (check multiple extensions)
    LATEST_LOG=""
    for ext in "*.log" "*.txt" "Player.log" "output_log.txt"; do
        FOUND_LOG=$(find "$LOG_DIR" -name "$ext" -type f -printf '%T@ %p\n' 2>/dev/null | sort -n | tail -1 | cut -d' ' -f2-)
        if [ -n "$FOUND_LOG" ]; then
            LATEST_LOG="$FOUND_LOG"
            break
        fi
    done
    
    if [ -z "$LATEST_LOG" ]; then
        print_error "No log files found in $LOG_DIR"
        echo ""
        echo "Available files in directory:"
        ls -la "$LOG_DIR" 2>/dev/null || echo "  (Cannot list directory contents)"
        echo ""
        echo "Please start the game at least once to generate log files."
        return 1
    fi
    
    print_info "Latest log file: $(basename "$LATEST_LOG")"
    echo ""
    
    # Check for AI Agent Plugin messages
    echo "🔍 Searching for AI Agent Plugin messages..."
    echo ""
    
    if grep -q "\[AI Agent Plugin\]" "$LATEST_LOG" 2>/dev/null; then
        print_status "Found AI Agent Plugin messages:"
        echo ""
        grep "\[AI Agent Plugin\]" "$LATEST_LOG" | tail -20
        echo ""
        
        # Count messages
        COUNT=$(grep -c "\[AI Agent Plugin\]" "$LATEST_LOG" 2>/dev/null)
        print_info "Total AI Agent Plugin messages: $COUNT"
    else
        print_error "No AI Agent Plugin messages found in the latest log file."
        echo ""
        echo "This could mean:"
        echo "  - The mod is not enabled"
        echo "  - The mod is not loading properly"
        echo "  - The game hasn't been started since enabling the mod"
        echo ""
        echo "Recent log entries (last 10 lines):"
        tail -10 "$LATEST_LOG" 2>/dev/null || echo "  (Cannot read log file)"
    fi
    
    echo ""
    print_info "End of Log Check"
}

# Function to clean build artifacts
clean_build() {
    echo "=== Cleaning Build Artifacts ==="
    echo ""
    
    if [ -d "bin" ]; then
        rm -rf bin
        print_status "Removed bin directory"
    fi
    
    if [ -d "obj" ]; then
        rm -rf obj
        print_status "Removed obj directory"
    fi
    
    print_status "Clean complete!"
}

# Function to run full build and test
run_full() {
    echo "=== Full Build and Test ==="
    echo ""
    
    build_mod
    echo ""
    test_installation
    echo ""
    test_ollama
    echo ""
    
    print_status "Full build and test completed successfully!"
    echo ""
    print_info "Your AI Agent Plugin is ready to use!"
    echo ""
    print_info "Quick start:"
    echo "1. Run '$0 install' to install the mod"
    echo "2. Start Oxygen Not Included and enable the mod"
    echo "3. Look for the green 'AI' button in the game toolbar"
    echo "4. Run '$0 logs' to check if mod is loading properly"
}

# Main script logic
case "${1:-build}" in
    "build")
        build_mod
        ;;
    "test")
        test_installation
        ;;
    "ollama")
        test_ollama
        ;;
    "install")
        install_mod
        ;;
    "logs")
        check_logs
        ;;
    "clean")
        clean_build
        ;;
    "full")
        run_full
        ;;
    "help"|"-h"|"--help")
        show_usage
        ;;
    *)
        print_error "Unknown command: $1"
        echo ""
        show_usage
        exit 1
        ;;
esac
