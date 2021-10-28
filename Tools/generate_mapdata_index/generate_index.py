import os
import json

curdir = os.path.abspath(os.path.join(os.curdir, "../../Assets/Res/mapdata"))
normalFile = os.path.join(curdir, "normal_index.json")

files = os.listdir(curdir)

print(curdir)

li = []

def Add(v, name):
	for i in range(len(li)):
		if v < li[i]["id"]:
			li.insert(i, { "id":v, "file":name, "info":"" })
			return
	li.append({ "id":v, "file":name, "info":"" })

log = ""
for file in files:
	if file.endswith(".json"):
		if (file.startswith("map")):
			p = file.index(".json")
			Add(int(file[3:p]), file[0:p])
			log = log + file[3:p] + ", "

print(log)

src = json.dumps(li)

file = open(normalFile, "w")
file.write(src)
file.close()