(function (cjs, an) {

var p; // shortcut to reference prototypes
var lib={};var ss={};var img={};
lib.ssMetadata = [
		{name:"smallCanvas_atlas_1", frames: [[260,1082,172,322],[0,1082,258,306],[0,0,1920,1080]]}
];


(lib.AnMovieClip = function(){
	this.actionFrames = [];
	this.ignorePause = false;
	this.gotoAndPlay = function(positionOrLabel){
		cjs.MovieClip.prototype.gotoAndPlay.call(this,positionOrLabel);
	}
	this.play = function(){
		cjs.MovieClip.prototype.play.call(this);
	}
	this.gotoAndStop = function(positionOrLabel){
		cjs.MovieClip.prototype.gotoAndStop.call(this,positionOrLabel);
	}
	this.stop = function(){
		cjs.MovieClip.prototype.stop.call(this);
	}
}).prototype = p = new cjs.MovieClip();
// symbols:



(lib.CachedBmp_4 = function() {
	this.initialize(ss["smallCanvas_atlas_1"]);
	this.gotoAndStop(0);
}).prototype = p = new cjs.Sprite();



(lib.CachedBmp_3 = function() {
	this.initialize(ss["smallCanvas_atlas_1"]);
	this.gotoAndStop(1);
}).prototype = p = new cjs.Sprite();



(lib.map_img = function() {
	this.initialize(ss["smallCanvas_atlas_1"]);
	this.gotoAndStop(2);
}).prototype = p = new cjs.Sprite();



(lib.volumeknob = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// timeline functions:
	this.frame_2 = function() {
		playSound("volume");
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).wait(2).call(this.frame_2).wait(2));

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f("rgba(0,255,0,0.588)").s().p("AgRD3IgOAAIgBAAIgLgBIgLgBIgKgDIgLgCIgKgDIgDgBIgBAAIAAAAIgBgBIgBAAIgBAAIgBgBIgCAAIgCAAIgDgBIgCAAIgCAAIgBAAIgBgBIgBAAIgCgBIgCAAIgCgBIgCgBIgBAAIAAAAIgBAAIgIgCIgHgDIgGgCIgGgDIgGgEIgDgCIgDgBIgEgCIgDgCIgEgDIgDgBIgBgBIgSgJQgIgEgHgGQgGgFgGgIIgHgKIgGgPIgDgJQgKgJgHgJIgKgQQgFgIgBgJQgDgPAAgOIAAgbQAAgOAFgNQAEgMAHgLIAHgLIADgFIAEgFIAEgFIAFgEIAEgEIADgCIACgBIACgBIAEgEIAFgCIAFgCIAEgCIAFgCIACgCIABAAIAAAAIAAgMIAAgKIABgKIACgLIACgKIABgOIADgOIADgKIAFgKIAGgJIAGgGIABgBIABgBIABgBIABgBIABgCIABgBIABgBIAAgBIAGgFIAFgFIAGgDIAFgFIAGgDIADgBIACgBIACgBIACgBIACgBIABgBIABAAIAEgDIAGgBIAEgCIAFgCIAEgCIAAAAIAFgCIAHgCIAGgCIAFgBIAGgBQAKgDAKgBIARAAIAUAAIAQABIARACIAFAAIAEABIAEAAIAEABIADABIACAAIACABIACAAIACAAIACABIACAAIACABIABAAIAAAAIABAAIABAAIABAAIABABIABAAIAHABIAGACIAGACIAEACIAEABIACACIAQAIIAMAJIAKALIAJANQAFAHADAHIABABIAAACIABABIAAAAIABABIAAABIACACIACAEIACADIACADIACADIABACIADABIADACIADACIADADIADACIABABIADAAIACABIADACIACACIACACIACABIAIAGIAGAEIAGAGIAFAGIAFAHIABACIADADIADAEIACAEIACADIADACIABACQAFAGAEAIIAGANIAEANIABANIABAVIAAAFQAEAIACALQACAKgBAKQgBAIgCAJQgDALgFAJQgEAJgGAIIgBAAIgCAFIgDAGIgDAEIgEAFIgEADIgDAEIAAABIgBACIgBADIgCADIgBACIgBABIgFAKIgGAJIgGAIIgHAIIgHAHIgCABIgEAEIgFAFIgEADIgFADIgFADIgDABIgDACIgEADIgDACIgEABIgDACIgCABIgBAAIAAAAIgBABIgBAAIgBABIgBABIgBAAIAAABIgBAAIAAABIgFACIgGACIgGACIgFADIgFABIgHABIgBAAIgEABIgEACIgEABIgDABIgCACIgBAAIgFABIgBAAIgDACIgFABIgFABIgEABIgEABIgEAAIgEABIgDABIgEABIgDABIgBAAIgOAFIgNADIgNABIgOAAgABwjYIAAAAIAAAAg");
	this.shape.setTransform(55.5563,24.65);
	this.shape._off = true;

	this.timeline.addTween(cjs.Tween.get(this.shape).wait(3).to({_off:false},0).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(0,0,82.4,49.3);


(lib.tremolo_1 = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// timeline functions:
	this.frame_2 = function() {
		playSound("tremolo");
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).wait(2).call(this.frame_2).wait(2));

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f("rgba(0,255,0,0.588)").s().p("Ax2GmQgEgEgDgHQgGgNgDgKQgGgUAEgbQACgTgBgHIgEgVQgCgJABgZIACgNQAAgGADgKQAJgZAGgLQAJgUAOgLQAPgMAdgIQAwgOBMgFQANgDAZAAIABAAIAagEIATgCQAqgKAZABIAMgBIANgFQALgDAMADIANAEIAMAAIAegBIAHAAIAtgGQAjgFAWgBIABAAQAugKAegDQAPgCAbAAIApgCQAcgCAqgHQDWggC/gMQChgLAmgFQBxgLBRgdQATgGAvgTIAkgPIAOgJIACgCQALgJAZgNIAAgEIACgLIACgHIAJgMIACgHQAEgFAKgIQAXgSAtgaQAzgdATgOIAQgLQASgRAZgSQARgMAngYQA1ghAhgJIADgBIAFgDQAdgRAbgFIAPgCIADgCQAngSAqgDQAYgBAJAJQAJAJAAATQABAVgGAMQgEAJgJAIIgSAPIgdAZIgdAZQgUAQghAVIhIAuQgbASgQAIQgQAIgbALIgrASQgnAUgVAJQghANhFAMQgFAEgEABQgDABgFgBIgJgBIgDAAIgaARIgLAGIgHABIgBAAQgDACgEgBQg8AigsAPQgaAJg3AOQgvAMgYADQgRADgbABIgsADQgTABg9AJQhjAOhWAFIhFADQgoACgdADQgwAFhJANIh5AVQiDAVixAHIgOABQgQABgUAAIgDAAIg3AGIgdAAQgMgBgJACIgMACIgFgBIgBAAIg+ANQgfAHgVAEIgDABIg6ANQgHACgFAAQgJAGgPAVIgEAFIAAAFIgDBHQAAAPACALIADAPIABAbIABAOQgCAJgGACQABAKgIAGQgEACgFAAQgFAAgDgCgAMHjAIAAACIALgHIgLAFg");
	this.shape.setTransform(116.5127,42.4202);
	this.shape._off = true;

	this.timeline.addTween(cjs.Tween.get(this.shape).wait(3).to({_off:false},0).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(0,0,233,84.9);


(lib.toneknobs = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// timeline functions:
	this.frame_2 = function() {
		playSound("tone");
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).wait(2).call(this.frame_2).wait(2));

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f("rgba(0,255,0,0.588)").s().p("AiTKLIgKAAIgIAAIgLAAIgMAAIgLAAIgLgCIgLgDIgDgBIgLgBIgLgDIgPgHIgKgFIgJgGIgCgBIgBgBIgCgBIgCgBIgBgCIgCgBIgEgCIgDgCIgDgDIgFgDIgFgFIgDgDIgDgCIgDgCIgDgEIgDgCIgCgDIgCgCIgDgDIgCgCIgDgDIgDgDIgCgDIgBgBIgDgEIgCgDIgCgDIgCgDIgCgDIgCgBIgDgFIgDgDIgDgEIgCgEIgDgFIAAgCIgEgGIgDgGIgDgHIgCgHIgCgJIAAgDQgDgKgBgKQgBgOAAgPQAAgKACgKIAEgUIAHgVIABgFIACgEIACgEIACgDIACgEIACgEIABgCIABgBIABgBIABgCIABgCIABgCIABAAIAAgBIABgBIABgBIAAgBIABgCIABgBIABgCIABgBIAEgFIADgFIAEgFIAFgFIAGgHIACgKIAEgNIAIgNIAHgKIALgJIABgBIAAgBIACgDIACgDIACgCIACgEIADgCIADgDIAGgIIAHgIIAEgFIAFgGIAIgIIAHgGIAFgEIAGgGIAIgFIAKgFIALgFIAJgBIALgEIANgCIAPgCIAMgBIANAAQAIgEAKgCIAXgFIAUgBQAKABAJADIASAHIABABIABAAIABAAIABAAIABABIABAAIABAAIAFACIAFABIAFAAIAGACIAIACIABABIAGABIAFABIAGACIAHADIAIAFIABABIAEACIAEACIAEADIADACIACACIAFAEIADADIADACIADACIADADIADADIADADIABAAIABABIABABIAAAAIABABIABABIAAAAIABAAIAEACIADACIAEADIADADIAEADIADAFIABABIACACIABAAIABACIABABIABACIABABIABABIACACIABAAIABABIABABIAAAAIABABIAAABIAAAAIAAADIAAAAIAAAAIAEAGIAFAHIAFALIADALIACAKIABADIABAFIABAEIABAFIAAAEIABAGIAAADIABAJIAAAQIAAATQABAHgBAHIgDALIAAAEIAAAGIAAAEIgBAFIgBAFIAAADIgBADIAAAEIgBAEIgBAEIgBAEIgBABIAAACIAAACIgBADIAAABIAAACIgBABIAAACIgBAFIgBAEIgBAFIgCAEIgCAFIgCADIAAACIgBACIgBACIgBABIgBACIgBADIgBABIAAABIgCAFIgDAFIgDAEIgDAFIgFAHIgCACIAAABIgBABIgBACIgCACIgBABIgBACIgBABIgBAAIgDAFIgDADIgEADIgDAEIgFADIgBABIgDAEIgEADIgEAEIgEADIgFAEIgDADIgCACIgCACIgCABIgDACIgCACIgBACIgDACIgDADIgEADIgFADIgFACIgDACIgBAAIgBABIAAABIgBABIgBAAIgBABIgBAAIAAAAIAAAAIAAABIAAAAIAAAAIgBAAIgBAAIgBAAIgBABIgBABIgBAAIgDABIgCABIgBABIgBABIgEABIgDACIgEABIgEACIgGADIAAABIgEACIgEACIgFACIgEACIgGACIgCAAIgBABIgCABIgCAAIgBABIgCAAIgCABIgCAAIgBABIgEACIgDABIgEACIgEAAIgEABIgBAAIgHACIgHACIgIABIgKABIgHABIgHABIgHAAIgIAAgABqi1IgEAAIgDAAIgJAAIgJAAIgJAAIgJgBIgIgCIgBAAIgEgBIgEAAIgDgBIgEgBIgDgBIgEgCIgHgBIgIgCIgHgCIgFgEIgHgDIgGgDIAAAAIgBgBIgBAAIgBgBIgBgBIgBAAIAAgBIgBAAIgBAAIAAgBIgBAAIgBAAIgBgBIAAAAIgBgBIgDgBIgDgCIgDgCIgDgCIgCgCIgDgCIgBgBIgGgFIgGgHIgGgGIgFgGIgFgIIgCgDIgCgDIgBgEIgCgCIgBgDIgCgDIgBgCIAAgBIAAgBIAAAAIgBgBIAAgBIgCgDIgBgCIgCgEIgBgDIgBgCIgBgEIgBgCIgCgEIgBgFIgCgEIgBgGIgBgEIAAgGIAAgBIgBgEIgBgDIAAgDIgBgEIgBgEIAAgEIgCgJIgBgJIgBgJIAAgJIAAgKIAAgDIgBgLIAAgPQAAgHABgGIACgOQABgHADgHIACgGIACgEIABgDIABgDIACgEIABgDIADgDIAAAAIAGgKIAIgKIAJgKIAJgHIAKgHIAHgEIABgBIAAgBIAAAAIABgBIABgDIABgEIADgDIACgEIACgDIACgDIAFgIIAGgIIAIgJIAFgGIAGgFIAHgFIACgCIAAAAIABAAIABgBIACgCIACgCIACgCIACgBIACgBIABgBIADgCIADgDIAEgCIADgCIAEgCIADgCIACgCIADgBIADgCIADgBIACgBIACgBIABgBIACAAIACgBIANgEIANgEIANgBIAPAAQAHAAAGABIAHADIABAAIADAAIACABIAEABIADABIADABIACABIAEABIAEABIAEACIAEACIAEABIADACIARgHIAJgDIAOgBIARAAIAXAAQAKABAKACIAOAGIgBAAIAEADIAJAGIAIAHIAHAHIAGAHIAGAJIABACIABABIACACIABACIACACIABACIABADIACABIACADIABAEIACACIACAEIABADIABABIABACIABABIABADIAAABIABACIAAABIACACIABADIABACIABADIABAEIABADIABABIAAAAIAAABIABABIAAABIAAABIABABIAAABIAAABIABABIAAAAIABACIAAABIABACIAAABIAAABIABABIAAABIAAAAIAAABIAAABIADALIADAMIABALIABANIABAJIAAAEIABACIAAACIABADIAAADIAAAEIAAAEIACATQAAAKgCAJIgCAUIgDATQgCAMgEALQgEAJgFAIIgEAIIAAAAIgBABIAAABIgBAAIAAABIgBABIAAAAIgDAGIgDAFIgEAFIgFAGIgFAFIgEADIgCACIgBACIgCACIgCABIgDACIgBACIgBABIgEADIgFAFIgEADIgFAFIgEADIgDACIgBABIgBACIgCACIgBABIgCACIgBAAIAAABIgBABIgBAAIAAABIgDAAIAAgBIgCACIgBABIgCABIgBABIgCABIAAAAIgBAAIgCABIgFADIgGAEIgGADIgGADIgGADIgEABIgCABIgCABIgBAAIgCABIgCAAIgBABIgCAAIgBABIgFACIgGACIgGACIgGACIgGACIgEABIAAAAIgDABIgCABIgDAAIgEABIgEABIgFABIgEAAIgCABIgBAAIgCABIgCABIgBAAIgCAAIgBAAIgDABIgDABIgEABIgDAAIgEAAIgDAAIgBAAIgDABIgCAAIgDABIgCAAIgDABIgDAAIgCAAIgDABIgEAAIgEAAIgFAAg");
	this.shape.setTransform(38.9688,149.95);
	this.shape._off = true;

	this.timeline.addTween(cjs.Tween.get(this.shape).wait(3).to({_off:false},0).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(0,0,78,215.1);


(lib._switch_1 = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// timeline functions:
	this.frame_2 = function() {
		playSound("_switch");
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).wait(2).call(this.frame_2).wait(2));

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f("rgba(0,255,0,0.588)").s().p("Ah0EuIgHAAIgPgBQgHgBgGgDIgLgIQgFgFgEgGQgEgHgCgHIgBgDQgCgDAAgEIgCgIIABgIIABgHIAEgIIACgDIAAgCIAAgBIABgCIAAgCIABgCIAAgBIABgCIADgHIAFgIIAGgHIAHgFQAFgDAFgCIAKgBIABgCIACgBIABgBIABgBIACgBIABgBIADgBIACgBIACgBIACAAIACgCIABgBIACgBIABgBIACgBIACgBIABgCIAEgCIAGgCIAGgCIAGgCIAHAAIAEgBIABgBIABgCIABgCIABgBIAAgCIABgEIABgEIABgDIACgEIADgEIADgDIABgCIAAgCIABgCIAAgCIABgCIAAgCIABgBIABgCIAAgCIABgCIAAgCIABgCIABgCIABgCIABgCIABgBIAAgCIABgCIABgCIABgCIAAgCIAAgCIABgCIAAgCIABgCIABgCIAAgBIABgCIABgCIAAgCIABgCIAAgBIABgCIABgCIAAgCIABgCIABgCIAAgEIABgDIAAgDIABgEIABgFIADgFIACgEIABgCIABgCIABgCIABgCIABgCIAAgEIABgEIACgDIACgEIACgDIADgEIACgCIABgBIABgCIAAgCIABgCIABgCIAAgCIABgEIABgEIABgBIABgCIACgDIABgBIABgGIADgFIABgEIADgEIACgFIAEgFIABgCIAAgCIABgBIABgCIABgCIABgBIABgCIABgBIABgCIABgCIAAgBIABgCIAAgCIABgBIAAgCIABgCIAAgCIABgCIABgBIABgCIABgCIACgDIABgBIABgCIABgBIAAgCIABgEIAAgEIABgEIABgDIABgEIACgDIACgDIABgBIAAgBIABgBIABgCIABgCIAAgCIAAgCIAAgDIABgCIAAgCIAAgCIABgBIABgCIAAgCIABgCIAAAAIABgCIAAgCIABgCIABgCIAAgBIABgCIABgCIABgBIAAgCIAAgCIABgCIAAgCIABgCIAAgCIABgBIAAgBIAAgCIABgCIAAgCIABgCIAAgCIABgBIABgCIABgCIABgCIABgCIABgBIAAgCIABgBIAAgCIACgDIABgDIABgEIABgEIACgEIACgDIACgEIABgCIABgCIABgCIABgCIAAgBIAAgBIABgCIAAgCIABgBIABgCIABgCIABgCIABgBIABgCIABgBIACgEIACgDIADgDIACgDIADgDIABgBIABgBIADgDIADgCIADgCIADgCIAEgBIAEgCQAHgCAIABQAIABAHADQAGAEAFAFQAGAHACAJQADAIgCAJIgBAEIgBACIAAACIgBACIgBACIgBACIgBABIgBACIgBACIgBABIgCABIgBACIgBABIgCABIAAABIAAACIgBACIgBACIgBACIgBABIgBACIgBACIgBACIgBABIAAACIgBACIgBABIgBACIgBABIAAACIgBACIgBABIAAACIgBABIgBADIAAACIgBABIAAACIgBABIgBACIgBABIAAACIgBAAIAAACIAAABIgBACIAAACIgBABIAAACIgBACIAAACIAAABIgBACIAAACIgBABIgBADIAAACIgBACIAAACIgBACIgBACIgBACIgBACIgBABIAAABIgBACIAAACIgBAGIgBAGIgCAFIgCAFIgCAFIgDAFIgBACIAAAFIgBAEIgBAEIgBAEIgBAEIgDAFIgBACIAAACIgBABIgBACIAAACIgBABIgBACIgCACIgBABIgBACIgBADIgBAEIgCAEIgCADIgBADIgCAEIgDADIAAACIgBACIgBACIgBACIgBACIgBACIgBACIgBABIgCACIAAAEIgBADIgCAEIgBAEIgDADIgCAEIgCACIgBACIAAABIgBADIgBADIgCAEIgBADIgCADIgCAEIgDADIAAACIAAACIgBACIAAACIgBABIgBACIgBACIgBACIgBABIgBADIgBACIgBABIAAACIgBACIgBABIAAAEIgBAEIgBAEIgBAEIgCAEIgBADIgBACIAAACIgBACIgBACIAAACIgBABIAAACIAAACIgBABIAAACIgBACIAAABIgBACIAAACIAAACIgBACIAAABIgBACIgBACIgBABIgCAEIgBABIgBACIAAACIgBACIgBABIAAACIgBACIAAAEIgBAEIgBAEIgBAEIgCAEIgBACIAAABIgBACIgBACIgBABIgBACIgBABIgBACIgBABIgBACIgBACIgBACIgBACIAAABIgBACIgBACIgBABIAAACIgBACIgBACIgBACIgBABIgBACIAAACIAAACIgBABIgBACIgBACIgBABIgBACIgBACIgBACIgBABIgBACIgBABIgBACIgBABIgCACIgBABIgBACIgBABIgBABIgCACIgBABIgCADIgCADIgCAEIgDADIgCADIgDADIgDACIgCAEIgCADIgCADIgDADIgDACIgDACIgBACIgBACIgBABIgCACIgBABIgBABIgCABIgBABIgGAIIgJAKIgLAIQgFADgHACIgLABIgEAAgACrjrIAAAAIgBABIABgBIABAAIAAgBIgBABg");
	this.shape.setTransform(18.2781,30.2013);
	this.shape._off = true;

	this.timeline.addTween(cjs.Tween.get(this.shape).wait(3).to({_off:false},0).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(0,0,36.6,60.4);


(lib.strings_1 = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// timeline functions:
	this.frame_2 = function() {
		playSound("strings");
	}
	this.frame_3 = function() {
		playSound("strings");
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).wait(2).call(this.frame_2).wait(1).call(this.frame_3).wait(1));

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("rgba(0,255,0,0.6)").ss(4,1,1).p("EhMLAliMCPAhHJQJTj3AEgD");
	this.shape.setTransform(499.3,270.825);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f().s("rgba(0,255,0,0.6)").ss(3,1,1).p("EBOIgnNItMGNI1NKoMh52A9m");
	this.shape_1.setTransform(518.7,273.45);

	this.shape_2 = new cjs.Shape();
	this.shape_2.graphics.f().s("rgba(0,255,0,0.6)").ss(5,1,1).p("EBKjgkLIl9CWMiPIBGB");
	this.shape_2.setTransform(477.1,267.325);

	this.shape_3 = new cjs.Shape();
	this.shape_3.graphics.f().s("rgba(0,255,0,0.6)").ss(2,1,1).p("EBSlgq0Ip0FSIrfFaMiOrBK9EBNbgpnIxEISMiO7BJs");
	this.shape_3.setTransform(556.175,281.225);

	this.shape_4 = new cjs.Shape();
	this.shape_4.graphics.f().s("rgba(0,255,0,0.6)").ss(1,1,1).p("EBT3gsgItnHZIrtFdMiOZBML");
	this.shape_4.setTransform(580.875,284.925);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[]}).to({state:[{t:this.shape_4},{t:this.shape_3},{t:this.shape_2},{t:this.shape_1},{t:this.shape}]},3).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-2.5,-1,1121.1,571.9);


(lib.connector = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// timeline functions:
	this.frame_2 = function() {
		playSound("jack");
	}
	this.frame_3 = function() {
		playSound("jack");
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).wait(2).call(this.frame_2).wait(1).call(this.frame_3).wait(1));

	// Слой_1
	this.instance = new lib.CachedBmp_4();
	this.instance.setTransform(0,0,0.5,0.5);
	this.instance._off = true;

	this.timeline.addTween(cjs.Tween.get(this.instance).wait(3).to({_off:false},0).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(0,0,86,161);


(lib.bridge_1 = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// timeline functions:
	this.frame_2 = function() {
		playSound("bridge");
	}
	this.frame_3 = function() {
		playSound("bridge");
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).wait(2).call(this.frame_2).wait(1).call(this.frame_3).wait(1));

	// Слой_1
	this.instance = new lib.CachedBmp_3();
	this.instance.setTransform(0,0,0.5,0.5);
	this.instance._off = true;

	this.timeline.addTween(cjs.Tween.get(this.instance).wait(3).to({_off:false},0).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(0,0,129,153);


// stage content:
(lib.smallCanvas = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.instance = new lib.toneknobs();
	this.instance.setTransform(360.85,398.55,0.6042,0.6042,0,0,0,0.1,0.1);
	new cjs.ButtonHelper(this.instance, 0, 1, 2, false, new lib.toneknobs(), 3);

	this.instance_1 = new lib.volumeknob();
	this.instance_1.setTransform(360.45,399.25,0.6042,0.6042,0,0,0,0.1,0.1);
	new cjs.ButtonHelper(this.instance_1, 0, 1, 2, false, new lib.volumeknob(), 3);

	this.instance_2 = new lib.strings_1();
	this.instance_2.setTransform(652.65,234.3,0.6042,0.6042,0,0,0,557.9,285.1);
	new cjs.ButtonHelper(this.instance_2, 0, 1, 2, false, new lib.strings_1(), 3);

	this.instance_3 = new lib.connector();
	this.instance_3.setTransform(318.35,506.65,0.6042,0.6042,0,0,0,43.1,80.5);
	new cjs.ButtonHelper(this.instance_3, 0, 1, 2, false, new lib.connector(), 3);

	this.instance_4 = new lib.tremolo_1();
	this.instance_4.setTransform(399.45,405.95,0.6042,0.6042,0,0,0,116.6,42.6);
	new cjs.ButtonHelper(this.instance_4, 0, 1, 2, false, new lib.tremolo_1(), 3);

	this.instance_5 = new lib.bridge_1();
	this.instance_5.setTransform(327.85,395.7,0.6042,0.6042,0,0,0,64.7,76.5);
	new cjs.ButtonHelper(this.instance_5, 0, 1, 2, false, new lib.bridge_1(), 3);

	this.instance_6 = new lib._switch_1();
	this.instance_6.setTransform(440.15,441.5,0.6042,0.6042,0,0,0,18.3,30.3);
	new cjs.ButtonHelper(this.instance_6, 0, 1, 2, false, new lib._switch_1(), 3);

	this.instance_7 = new lib.map_img();
	this.instance_7.setTransform(0,0,0.6042,0.6042);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.instance_7},{t:this.instance_6},{t:this.instance_5},{t:this.instance_4},{t:this.instance_3},{t:this.instance_2},{t:this.instance_1},{t:this.instance}]}).wait(1));

	this._renderFirstFrame();

}).prototype = p = new lib.AnMovieClip();
p.nominalBounds = new cjs.Rectangle(580,326,580,326.5);
// library properties:
lib.properties = {
	id: 'F9ED657EF4323348813A563B4422CB7B',
	width: 1160,
	height: 652,
	fps: 30,
	color: "#FFFFFF",
	opacity: 1.00,
	manifest: [
		{src:"images/smallCanvas_atlas_1.png?1749990965122", id:"smallCanvas_atlas_1"},
		{src:"sounds/bridge.mp3?1749990965167", id:"bridge"},
		{src:"sounds/jack.mp3?1749990965167", id:"jack"},
		{src:"sounds/strings.mp3?1749990965167", id:"strings"},
		{src:"sounds/_switch.mp3?1749990965167", id:"_switch"},
		{src:"sounds/tone.mp3?1749990965167", id:"tone"},
		{src:"sounds/tremolo.mp3?1749990965167", id:"tremolo"},
		{src:"sounds/volume.mp3?1749990965167", id:"volume"}
	],
	preloads: []
};



// bootstrap callback support:

(lib.Stage = function(canvas) {
	createjs.Stage.call(this, canvas);
}).prototype = p = new createjs.Stage();

p.setAutoPlay = function(autoPlay) {
	this.tickEnabled = autoPlay;
}
p.play = function() { this.tickEnabled = true; this.getChildAt(0).gotoAndPlay(this.getTimelinePosition()) }
p.stop = function(ms) { if(ms) this.seek(ms); this.tickEnabled = false; }
p.seek = function(ms) { this.tickEnabled = true; this.getChildAt(0).gotoAndStop(lib.properties.fps * ms / 1000); }
p.getDuration = function() { return this.getChildAt(0).totalFrames / lib.properties.fps * 1000; }

p.getTimelinePosition = function() { return this.getChildAt(0).currentFrame / lib.properties.fps * 1000; }

an.bootcompsLoaded = an.bootcompsLoaded || [];
if(!an.bootstrapListeners) {
	an.bootstrapListeners=[];
}

an.bootstrapCallback=function(fnCallback) {
	an.bootstrapListeners.push(fnCallback);
	if(an.bootcompsLoaded.length > 0) {
		for(var i=0; i<an.bootcompsLoaded.length; ++i) {
			fnCallback(an.bootcompsLoaded[i]);
		}
	}
};

an.compositions = an.compositions || {};
an.compositions['F9ED657EF4323348813A563B4422CB7B'] = {
	getStage: function() { return exportRoot.stage; },
	getLibrary: function() { return lib; },
	getSpriteSheet: function() { return ss; },
	getImages: function() { return img; }
};

an.compositionLoaded = function(id) {
	an.bootcompsLoaded.push(id);
	for(var j=0; j<an.bootstrapListeners.length; j++) {
		an.bootstrapListeners[j](id);
	}
}

an.getComposition = function(id) {
	return an.compositions[id];
}


an.makeResponsive = function(isResp, respDim, isScale, scaleType, domContainers) {		
	var lastW, lastH, lastS=1;		
	window.addEventListener('resize', resizeCanvas);		
	resizeCanvas();		
	function resizeCanvas() {			
		var w = lib.properties.width, h = lib.properties.height;			
		var iw = window.innerWidth, ih=window.innerHeight;			
		var pRatio = window.devicePixelRatio || 1, xRatio=iw/w, yRatio=ih/h, sRatio=1;			
		if(isResp) {                
			if((respDim=='width'&&lastW==iw) || (respDim=='height'&&lastH==ih)) {                    
				sRatio = lastS;                
			}				
			else if(!isScale) {					
				if(iw<w || ih<h)						
					sRatio = Math.min(xRatio, yRatio);				
			}				
			else if(scaleType==1) {					
				sRatio = Math.min(xRatio, yRatio);				
			}				
			else if(scaleType==2) {					
				sRatio = Math.max(xRatio, yRatio);				
			}			
		}
		domContainers[0].width = w * pRatio * sRatio;			
		domContainers[0].height = h * pRatio * sRatio;
		domContainers.forEach(function(container) {				
			container.style.width = w * sRatio + 'px';				
			container.style.height = h * sRatio + 'px';			
		});
		stage.scaleX = pRatio*sRatio;			
		stage.scaleY = pRatio*sRatio;
		lastW = iw; lastH = ih; lastS = sRatio;            
		stage.tickOnUpdate = false;            
		stage.update();            
		stage.tickOnUpdate = true;		
	}
}
an.handleSoundStreamOnTick = function(event) {
	if(!event.paused){
		var stageChild = stage.getChildAt(0);
		if(!stageChild.paused || stageChild.ignorePause){
			stageChild.syncStreamSounds();
		}
	}
}
an.handleFilterCache = function(event) {
	if(!event.paused){
		var target = event.target;
		if(target){
			if(target.filterCacheList){
				for(var index = 0; index < target.filterCacheList.length ; index++){
					var cacheInst = target.filterCacheList[index];
					if((cacheInst.startFrame <= target.currentFrame) && (target.currentFrame <= cacheInst.endFrame)){
						cacheInst.instance.cache(cacheInst.x, cacheInst.y, cacheInst.w, cacheInst.h);
					}
				}
			}
		}
	}
}


})(createjs = createjs||{}, AdobeAn = AdobeAn||{});
var createjs, AdobeAn;