using GCL.Pattern;
using UnityEngine;

public class UiMessage_OnButtonRankMode : MessageBase<UiMessage_OnButtonRankMode> {}
public class UiMessage_OnButtonInfiniteMode : MessageBase<UiMessage_OnButtonInfiniteMode> {}
public class UiMessage_OnButtonChallengeMode : MessageBase<UiMessage_OnButtonChallengeMode> {}

public class UiMessage_OnButtonGameAgain : MessageBase<UiMessage_OnButtonGameAgain> {}
public class UiMessage_OnButtonGameBack : MessageBase<UiMessage_OnButtonGameBack> {}
public class UiMessage_OnClassicLevelSelect : MessageBase<UiMessage_OnClassicLevelSelect> {
	public UiMessage_OnClassicLevelSelect(int mapID) {
		this.mapID = mapID;
	}
	public int mapID { get; private set; }
}