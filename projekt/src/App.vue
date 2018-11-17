<template>
  <div id="app">
	<h2>{{title}}</h2>
	<my-table :time='time' :rows='rows'></my-table>
	<add-form :dat='emptyobject' @addFunction="addfunction" />
  </div>
</template>

<script>
import table from './VueGoodTable.vue'
import form from './AddForm.vue'

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
    objDat = new Date();
	dat.hour = FormatNumberLength(objDat.getHours(),2);
	dat.min = FormatNumberLength(objDat.getMinutes(),2);
	dat.sec = FormatNumberLength(objDat.getSeconds(),2);
 }, 1000);
 
 
 var data1 = [
	];
	
 setInterval(function(){
	let time = FormatNumberLength(objDat.getHours(),2);
	time+=":";
	time+=FormatNumberLength(objDat.getMinutes(),2);
	
	data1.forEach(x=> {
		if(x.time === time)
		{
			alert("Godzina:"+x.time+"\nzdarzenie:"+x.task+"\npowiadomienie:"+x.msg);
		}});
 }, 60000);
 
var regex = new RegExp("^([0-1][0-9]|2[0-3]):([0-5][0-9])$");
 
var myobj = {id:0, time:"",task:"",topic:"", msg:"",IsValidTime:function(){return regex.test(this.time);}};
 
export default {
  name: 'app',
  data () {
    return {
	title:"my App",
	time:dat,
	rows: data1,
	emptyobject: myobj,
	addfunction: function()
	{
		if(myobj.IsValidTime())
		{
			data1.push(JSON.parse(JSON.stringify((myobj))));
		}
	}
	}
  },
  components: {
  'my-table':table,
  'add-form':form
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
