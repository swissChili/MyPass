dotnet publish --configuration release
sudo cp bin/release/netcoreapp3.0/publish /opt/MyPass -r
echo 'add /opt/MyPass to your path'
echo 'PATH=$PATH:/opt/MyPass'
