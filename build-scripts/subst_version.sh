#! /bin/bash
find -type f \( -name '*.csproj' -o -name '*.cs' \) -exec sed -i "s/0.999.0/$1/g" {} \;