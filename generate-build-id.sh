#!/bin/bash

# Generate a new build ID and update the source code
echo "🔄 Generating new build ID..."

# Generate new UUID
NEW_UUID=$(uuidgen)
echo "🆔 New Build ID: $NEW_UUID"

# Update the source code with the new UUID
if [ -f "Source/AIAgentPlugin.cs" ]; then
    # Use sed to replace the BuildId line
    sed -i "s/public static string BuildId => \"[^\"]*\";/public static string BuildId => \"$NEW_UUID\";/" Source/AIAgentPlugin.cs
    
    if [ $? -eq 0 ]; then
        echo "✅ Updated AIAgentPlugin.cs with new build ID"
        echo "🔍 New build ID in source: $(grep "BuildId =>" Source/AIAgentPlugin.cs | cut -d'"' -f2)"
    else
        echo "❌ Failed to update AIAgentPlugin.cs"
        exit 1
    fi
else
    echo "❌ Source/AIAgentPlugin.cs not found!"
    exit 1
fi

echo "✅ Build ID generation complete!"
echo ""
echo "📝 Short ID (for logs): ${NEW_UUID:0:8}"
echo "📝 Full ID: $NEW_UUID"