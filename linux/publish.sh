#!/bin/bash
set -e

if [ ! -f "TweetDuck.Linux.sln" ]; then
	echo "Missing solution file in working directory!"
	exit 1
fi

mkdir -p bld/tmp/

dotnet build TweetDuck -c Release -r linux-x64 -o bld/tmp/
rm bld/tmp/ref/*
rm -r bld/tmp/ref/

echo "Packaging build..."
tar czvf bld/TweetDuck.$("bld/tmp/TweetDuck" -appversion).tar.gz -C bld/tmp/ --owner=0 --group=0 --format=v7 .

rm -rf bld/tmp/

echo "Done!"
