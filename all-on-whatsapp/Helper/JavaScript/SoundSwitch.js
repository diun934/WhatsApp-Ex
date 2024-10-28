function muteAudio() {
    var videos = document.querySelectorAll('video');
    var audios = document.querySelectorAll('audio');
    videos.forEach(video => video.muted = true);
    audios.forEach(audio => audio.muted = true);
}
function unmuteAudio() {
    var videos = document.querySelectorAll('video');
    var audios = document.querySelectorAll('audio');
    videos.forEach(video => video.muted = false);
    audios.forEach(audio => audio.muted = false);
}
