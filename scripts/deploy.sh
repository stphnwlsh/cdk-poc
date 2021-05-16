#!/bin/bash
#!/usr/bin/env bash

echo "=================== Package Timestream Handler ==================="
cd ./src/TimestreamHandler

dotnet lambda package

cd ../..

echo "===================== Run Instrastructure ========================"
cd ./src/Infrastructure

cdk deploy --all --require-approval never

cd ../..
