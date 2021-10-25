# 依赖项: FFmpeg
# 使用方法:
# 	将音频放入当前目录,执行脚本

import os

curdir = os.path.abspath(os.curdir)
outdir = os.path.normpath(curdir + "/output")

def removedir(dir):
	if os.path.exists(dir):
		for i in os.listdir(dir):
			f = os.path.join(dir, i)
			if os.path.isdir(f):
				removedir(f)
			else:
				os.remove(f)
		os.rmdir(dir)

if os.path.exists(outdir):
	removedir(outdir)

os.mkdir(outdir)

for item in os.listdir(curdir):
	if os.path.isfile(item):
		if item.endswith(("wav", "mp3", "aac", "flac")):
			stt = os.path.splitext(item)
			dest = stt[0] + ".ogg"
			cmd = "ffmpeg -i \"" + os.path.join(curdir, item) + "\" -acodec libvorbis \"" + os.path.join(outdir, dest) + "\""
			print("> " + cmd)
			os.system(cmd)
