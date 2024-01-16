
enum 公司 {
	安布雷拉,
	符文科技,
	问山宗,
	深海母巢,
	燃烧军团
}

//----诸天交易所的变量
let {
	//世界运行计时器
	timer = 0,
	//交易价格刷新间隔
	price_cd = 0,
	//软妹币
	rmb = 5000 + get_num(100),
	price = [
		//安布雷拉初始价
		220 + get_num(10),
		//符文科技初始价
		320 + get_num(10),
		//问山宗初始价
		290 + get_num(10),
		//深海母巢初始价
		340 + get_num(10),
		//燃烧军团初始价
		260 + get_num(10)
	],
	own = [
		//持有安布雷拉数量
		0,
		//持有符文科技数量
		0,
		//持有问山宗数量
		0,
		//持有深海母巢数量
		0,
		//持有燃烧军团数量
		0
	],
	//买入数量
	buy_quantity = 1,
	//总成本
	TotalCost = 0
} = World.getSaveValue("诸天交易所", "data", {}) as {
	timer: number,
	price_cd: number,
	rmb: number,
	price: number[],
	own: number[],
	buy_quantity: number,
	TotalCost: number
};

//----末世异闻录的变量
let Hero_hp = 100;//主角体力
let deadline = 86400;//时间病毒
let Act = 1;//当前关卡
let Metal_waste = 0;//金属破烂
({
	Hero_hp,
	deadline,
	Act,
	Metal_waste
} = World.getGlobalSaveValue("末世异闻录", "data", {
	Hero_hp,
	deadline,
	Act,
	Metal_waste
}));

let temp_1 = "风平浪静，无事发生……";
let print = (text: string) => World.addRightText(text + "\n");

World.setTitle("[center][color=#FFFF00] 诸天交易所v2 [/color] \n[font_size=14]这就是俺的外挂？这玩意儿在末世能有用嘛？（模板世界随意魔改，请勿怜惜）[/font_size][/center]");
World.setBackgroundTexture("背景1.jpg");
print(temp_1);

World.event.once("ready", () => {
	更新行情();
	更新持有();
	更新事件();
	setInterval(main_loop, 1000);
});

World.event.on("left_button_click", (text: string, id: number) => {
	if (text == "掀桌子重来") {
		own = [0, 0, 0, 0, 0];
		TotalCost = 0;
		rmb = 2333;
		更新持有();
		after();
		print("你恼羞成怒..把世界重启了..");
	}
});

World.event.on("text_url_click", (meta: string, id: number) => {
	if (id == 0) {
		买入(公司[meta]);
	} else if (id == 1) {
		卖出(公司[meta]);
	}
});

//获取1个随机数
function get_num(n: number): number {
	return Math.floor((Math.random() * 8 * n) - 4 * n); //在-4*n到4*n之间随机取值
}

function main_loop(): void {
	//计时器自增,
	timer++;
	//每次系统级刷新CD时调用一次,使交易价格刷新进度+1（刷新基数为1秒时，则每秒进度+1）
	price_cd++;
	//当刷新进度>=10时,即每10秒刷新一次股市价格，并重置进度
	if (price_cd >= 10) {
		price_cd = 0;
		price[0] += get_num(2);
		price[1] += get_num(4);
		price[2] += get_num(4);
		price[3] += get_num(5);
		price[4] += get_num(3);
	}
	更新行情();
	更新持有();
	更新事件();
	World.setSaveValue("诸天交易所", "data", {
		timer,
		price_cd,
		rmb,
		price,
		own,
		buy_quantity,
		TotalCost
	});
}

function 更新行情(): void {
	World.setLeftText(
		`[center]
累计营业 ${timer}秒
${10 - price_cd}秒后行情更新

[color=#7FFF00]-股市行情-[/color]
${公司[0]} ￥${price[0]} [color=#ff00ff][url=${公司[0]}]买入[/url][/color]
${公司[1]} ￥${price[1]} [color=#ff00ff][url=${公司[1]}]买入[/url][/color]
${公司[2]} ￥${price[2]} [color=#ff00ff][url=${公司[2]}]买入[/url][/color]
${公司[3]} ￥${price[3]} [color=#ff00ff][url=${公司[3]}]买入[/url][/color]
${公司[4]} ￥${price[4]} [color=#ff00ff][url=${公司[4]}]买入[/url][/color]
[/center]`
	);
	更新持有();
}

function 更新持有(): void {
	World.setCenterText(
		`[center]
[color=yellow]-流动资金-[/color]
软妹币 ￥ ${rmb}
[color=yellow]-总资产-[/color]
软妹币 ￥ ${rmb + own[0] * price[0] +
		own[1] * price[1] +
		own[2] * price[2] +
		own[3] * price[3] +
		own[4] * price[4]
		}

[color=yellow]-持有股票-[/color]
${公司[0]}.持有 ${own[0]} [color=#ff00ff][url=${公司[0]}]卖出[/url][/color]
${公司[1]}.持有 ${own[1]} [color=#ff00ff][url=${公司[1]}]卖出[/url][/color]
${公司[2]}.持有 ${own[2]} [color=#ff00ff][url=${公司[2]}]卖出[/url][/color]
${公司[3]}.持有 ${own[3]} [color=#ff00ff][url=${公司[3]}]卖出[/url][/color]
${公司[4]}.持有 ${own[4]} [color=#ff00ff][url=${公司[4]}]卖出[/url][/color]
[color=yellow]-持股账面总值-[/color]
软妹币 ￥${own[0] * price[0] +
		own[1] * price[1] +
		own[2] * price[2] +
		own[3] * price[3] +
		own[4] * price[4]
		}

[color=yellow]-持仓总成本-[/color]
软妹币 ￥${TotalCost}
[/center]`
	);
	after();
}

function 更新事件(): void {
	if (timer % 10 == 0) {
		let rr = get_num(3);
		//此处if仅作演示之用,具体需求具体分析.
		if (rr == 5) {
			temp_1 = `A病毒与V病毒的融合研究有了新突破！${公司[0]}股价上涨！`;
			price[0] += 12;
		} else if (rr == 4) {
			temp_1 = `经典冷饭款符文震动棒Pro Max Plus销量大涨！${公司[1]}股价上涨！`;
			price[1] += 22;
		} else if (rr == 3) {
			temp_1 = `问道山挖挖峰弟子于秘境中发现银精矿藏！${公司[2]}股价上涨！`;
			price[2] += 24;
		} else if (rr == 2) {
			temp_1 = `母巢宣传部发展了一批高质量喷子信徒！${公司[3]}股价上涨！`;
			price[3] += 24;
		} else if (rr == 1) {
			temp_1 = `第四天灾如蝗虫过境般撤离军团驻地..${公司[4]}股价上涨！`;
			price[4] += 16;
		} else if (rr == -1) {
			temp_1 = `军团驻地遭受第四天灾日夜不停的骚扰..${公司[4]}股价下跌！`;
			price[4] -= 16;
		} else if (rr == -2) {
			temp_1 = `传奇调查员小帅闯入母巢三级实验区搞事！${公司[3]}股价下跌！`;
			price[3] -= 24;
		} else if (rr == -3) {
			temp_1 = `问道山逛逛峰弟子因意外激发上古传送大阵，导致群体失踪事件频发！${公司[2]}股价下跌！`;
			price[2] -= 12;
		} else if (rr == -4) {
			temp_1 = `因无法拿到大光球新品销售版号！${公司[1]}股价下跌！`;
			price[1] -= 22;
		} else if (rr == -5) {
			temp_1 = `因大量轮回者的频繁光顾，各地实验室损失不断！${公司[0]}股价下跌！`;
			price[0] -= 12;
		} else if (rr < -6) {
			if (timer > 300) {//不足300秒的新手免税
				temp_1 = "为激发交易活力，强化托盘力度，大光球有关部门决定启动接盘侠计划，随机征收现金税5%！";
				rmb -= Math.floor(rmb / 20);
			}
		} else {
			temp_1 = "风平浪静，无事发生……";
		}
		print(temp_1);
		after();
	}
}

function 买入(index: number): void {
	let 价格 = price[index];
	if (价格 > 0) {
		if (rmb >= 价格) {
			rmb -= 价格;
			own[index]++;
			TotalCost += 价格;
			更新持有();
			print(`${公司[index]}股票 购买成功`);
		} else {
			print(`软币不足，${公司[index]}股票 购买失败`);
		}
	} else {
		print(`${公司[index]}股票 停牌中`);
	}
	after();
}

function 卖出(index: number): void {
	let 价格 = price[index];
	if (价格 > 0) {
		let 股票 = own[index];
		if (股票 > 0) {
			rmb += 价格;
			own[index]--;
			更新持有();
			print(`${公司[index]}股票 卖出成功`);
		} else {
			print(`卖出失败，你未持有 ${公司[index]}股票`);
		}
	} else {
		print(`${公司[index]}股票 停牌中`);
	}
	after();
}

function after(): void {
	if (rmb < 500) {
		World.setBackgroundTexture("背景2.jpg");
		World.setLeftButtons(["掀桌子重来"]);
	} else {
		World.setBackgroundTexture("背景1.jpg");
		World.setLeftButtons([]);
	}
}