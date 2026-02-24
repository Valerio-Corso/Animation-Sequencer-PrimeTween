# Animation Sequencer

Based upon https://github.com/brunomikoski/Animation-Sequencer
This project is a fork of the original Animation Sequencer, which was designed to work with DOTween. 
This fork is simply a migration to PrimeTween for now.

## Features
- Allow you to create a complex sequence of Tweens/Actions and play on Editor Mode!
- User Friendly interface with a lot of customization
- Easy to extend with project specific actions
- Chain sequences and control entire animated windows with a single interface
- Searchable actions allowing fast interactions and updates
- Can be used for any type of Objects, UI or anything you want! 

## Built in Steps
 - Tween Target 
    - Anchored Position
    - Move
    - Scale
    - Rotate
    - Fade (Canvas Group)
    - Fade (Graphic)
    - Path
    - Shake (Position/Rotation/Scale)
    - Punch (Position/Rotation/Scale)
    - Text (TextMeshPro Support)
    - Fill  
 - Play Particle System
 - Play Animation Sequencer

## How to use?
- Animation Sequencer relies on PrimeTween, so install `com.kyrylokuzyk.primetween` (Asset Store or UPM). For UPM, add the npm scoped registry for `com.kyrylokuzyk` and install PrimeTween from My Registries.
- When the `PrimeTween.Runtime` assembly is detected, the `PRIMETWEEN_ENABLED` define is added automatically.
- Add the Animation Sequencer to any GameObject and start your animation! 
- Using the <kbd>+</kbd> button under the `Animation Steps` you can add a new step
- Select <kbd>Tween Target</kbd>
- Use the <kbd>Add Actions</kbd> to add specific tweens to your target
- Press play on the Preview bar to view it on Editor Time.
- To play it by code, just call use `animationSequencer.Play();`

## FAQ

<details>
<summary>I'm seeing errors like `The type or namespace name 'PrimeTween' could not be found`</summary> 
This means PrimeTween isn't installed or its asmdef wasn't detected. Install PrimeTween and make sure the `PrimeTween.Runtime` assembly is available so the `PRIMETWEEN_ENABLED` define can be set.
	
</details>
<details>
    
<summary>How can I create my custom actions?</summary> 
To create a custom action there's a few things you need to do, first your class needs to be `[Serializable]` in order to be properly displayed on inspector.
Now you need to make sure whatever you are doing, you are connecting it with the Sequence, like the example bellow.
Also notice that in this case I'm adding the Duration its getting the lenght from the clip

```c#
[Serializable]
 public class PlayLegacyAnimation : AnimationStepBase
 {
     public override string DisplayName => "Play Legacy Animation";

     [SerializeField]
     private Animation animation;

     public override void AddTweenToSequence(Sequence animationSequence)
     {
         animationSequence.ChainDelay(Delay);
         animationSequence.ChainCallback(
             () =>
             {
                 animation.Play();
             }
         );
         animationSequence.ChainDelay(animation.clip.length);
     }
 }
```

</details>

<details>

<summary>I have my own PrimeTween tweens, can I use that? </summary>

Absolutely! The same as the step, you can add any new tween action by extending `DOTweenActionBase` (legacy name). In order to avoid any performance issues all the tweens are created on the PrepareToPlay method on Awake, and are paused.

```c#
[Serializable]
public sealed class ChangeMaterialStrengthDOTweenAction : DOTweenActionBase
{
    public override string DisplayName => "Change Material Strength";
        
    public override Type TargetComponentType => typeof(Renderer);

    [SerializeField, Range(0,1)]
    private float materialStrength = 1;

    protected override Tween GenerateTween_Internal(GameObject target, float duration)
     {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer == null)
            return default;

        Material material = renderer.sharedMaterial;
        float start = material.GetFloat("_Strength");
        float end = materialStrength;
        return Tween.Custom(start, end, duration, value => material.SetFloat("_Strength", value), Ease.ToEasing());
        
    }
}
```

![custom-tween-action](https://user-images.githubusercontent.com/600419/109774425-3965a280-7bf8-11eb-9bfe-90b0be8b8617.gif)

</details>

<details>
    <summary>Using custom animation curve as easing </summary>
    
You can use the Custom ease to define an *AnimationCurve* for the Tween.
    
![custom-ease](https://user-images.githubusercontent.com/600419/109780020-7af94c00-7bfe-11eb-8f0f-52480dd97ea3.gif)

</details>

<details>
   <summary>What are the differences between the initialization settings</summary>
	
- <kbd>None</kbd> *Don't do anything on the AnimationSequencer Awake method*	
- <kbd>PrepareToPlayOnAwake</kbd> *This will make sure the Tweens that are from are prepared to play at the intial value on Awake.*
- <kbd>PlayOnAwake</kbd> Will play the tween on Awake.*
   
</details>

## System Requirements
Unity 2018.4.0 or later versions


## How to install

	
	
<details>
<summary>Add from OpenUPM <em>| via scoped registry, recommended</em></summary>

This package is available on OpenUPM: https://openupm.com/packages/com.brunomikoski.animationsequencer

To add it the package to your project:

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry:
  ```
  Name: OpenUPM
  URL:  https://package.openupm.com/
  Scope(s): com.brunomikoski
  ```
- click <kbd>Save</kbd>
- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `com.brunomikoski.animationsequencer`
- click <kbd>Add</kbd>
</details>

<details>
<summary>Add from GitHub | <em>not recommended, no updates :( </em></summary>

You can also add it directly from GitHub on Unity 2019.4+. Note that you won't be able to receive updates through Package Manager this way, you'll have to update manually.

- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `https://github.com/brunomikoski/Animation-Sequencer.git`
- click <kbd>Add</kbd>
</details>

