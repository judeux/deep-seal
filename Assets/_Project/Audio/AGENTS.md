# Imported audio scope

이 파일은 first-party music, SFX, ambience, AudioClip과 AudioMixer asset에 적용된다.
Root와 `Assets/_Project/AGENTS.md`의 proposal-only 경계를 유지한다.

- Audio spec은 trigger, purpose, duration, channel, sample rate/bit depth, loop,
  overlap, variation, leading silence와 tail을 명시한다.
- Default generated SFX master는 별도 spec이 없으면 WAV PCM 48 kHz, 24-bit다.
- Positionable world sound는 mono를 우선하고 stereo는 의도와 playback context를
  문서화한다.
- Clipping, unintended silence/noise, excessive limiting과 불필요한 baked reverb를
  허용하지 않는다.
- 반복 채굴, 공격, 피격과 UI SFX는 variation 또는 controlled pitch/volume 변화를
  정의한다.
- AudioMixer가 도입되면 기본 책임은 Master, Music, SFX, UI, Ambience로 분리하되
  현재 milestone에서 필요하기 전에는 asset을 미리 만들지 않는다.
- Arbitrary string path, broad singleton 또는 서로 다른 script의 global volume multiplier로
  재생·볼륨을 관리하지 않는다.
- Import/load/compression은 duration, frequency와 memory usage를 기준으로 정하고 실제
  gameplay volume과 overlap에서 검증한다.
