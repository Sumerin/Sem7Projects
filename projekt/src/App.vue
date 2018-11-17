<template>
  <div id="app">
	<h2>{{title}}</h2>
	<my-com :collection='time'></my-com>
  </div>
</template>

<script>
import clock from './clock.vue'

function FormatNumberLength(num, length) {
    var r = "" + num;
    while (r.length < length) {
        r = "0" + r;
    }
    return r;
}

var objDat = new Date();
var dat =
{
	hour : FormatNumberLength(objDat.getHours(),2),
	min: FormatNumberLength(objDat.getMinutes(),2),
	sec: FormatNumberLength(objDat.getSeconds(),2)
}

setInterval(function(){
 var objDat = new Date();
	dat.hour = FormatNumberLength(objDat.getHours(),2);
	dat.min = FormatNumberLength(objDat.getMinutes(),2);
	dat.sec = FormatNumberLength(objDat.getSeconds(),2);
 }, 1000);
 
export default {
  name: 'app',
  data () {
    return {
	title:"my App",
	time:dat
	}
  },
  components: {
  'my-com':clock
  }
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}

h1, h2 {
  font-weight: normal;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  display: inline-block;
  margin: 0 10px;
}

a {
  color: #42b983;
}
</style>
