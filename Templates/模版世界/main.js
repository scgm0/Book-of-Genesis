import Utils from "./lib.js";
import AudioPlayer from "audio";

let a = AudioPlayer.playFile("bgm.mp3");
World.print(a.finishedCallback = function(){
	World.print(this, this.currentPosition)
});

let t = Date.now();
let id = setTimeout((...a1)=>{
		World.print(...a1, (Date.now()-t)/1000);
		World.addRightButton("777");
	}, 1000, "a1", World.info.version);
World.setBackgroundTexture("icon.png");
World.event.on("ready", () => {
	// clearTimeout(id);
	World.print(import.meta.url, Date.now()-t);
	World.setBackgroundTexture(null);
	World.setBackgroundColor("#008080");
	t = Date.now();
});

World.event.on("left_button_click", (str, i) => {
	if(i == 0){
		World.addRightText(World.getSaveValue("test", "m1"));
	}
});

World.event.on("command", text => {
	World.setCommandPlaceholderText(text);
	World.setSaveValue("test", "m1", text)
});

World.event.on("right_button_click", (str, i) => {
	World.print(str);
	if(str == "777") {
		// World.removeLeftButton(-1);
		World.setLeftText(`
[img]icon.png[/img]
[i]好好好[/i]
[bgcolor=red][url={"data": "yes"}]确定[/url][url=no]取消[/url][/bgcolor]
[color=#ff00ff][url]无[/url][/color]
		`)
	}
});

World.event.on("text_url_click", (meta, i) => {
	switch(typeof meta){
		case "object":
			World.addRightText(`${meta?.data}, ${i}`);
			break;
		case "string":
			World.addRightText(`${meta}, ${i}`);
			break;
	}
	World.addRightText("\n");
})

let i = 0;
World.event.on("tick", () => {
	if(Date.now()-t <= 1000){
		i++
	} else {
		World.setTitle(`${World.info.name}-${Date.now()}`);
		World.addCenterText(`\n${(Date.now()-t)/1000}`);
		t = Date.now();
		i = 0;
	}
});

World.event.on("exit", e => {
	World.setBackgroundTexture(null);
	World.print(e, (Date.now()-t)/1000, i);
});

World.print(id, World.info.version);

World.print(World.setLeftButtons(["555", "666"]));

World.setCommandPlaceholderText("输入命令");

World.setCenterStretchRatio(2);

World.setCenterText("[b]金木水火土[/b]");

for(let i = 0; i < 10; i++){
	World.addRightButton(i**i)
}