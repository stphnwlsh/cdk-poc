#!/bin/bash
#!/usr/bin/env bash

echo "===================== Run Instrastructure ========================"
cd ./src/Infrastructure

cdk destroy --all --require-approval never

cd ../..
