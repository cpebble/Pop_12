build:
	fsharpc --nologo -a models.fs
	fsharpc --nologo -r models.dll 12i0.fsx 

run: build
	mono 12i0.exe
