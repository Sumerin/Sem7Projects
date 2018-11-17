<template>
<div>
<h2>Dodaj do terminarza</h2>
  czas:<br>
  <input type="text" v-model="dat.time" :class="[dat.IsValidTime()?'yes_class':'no_class']"><br>
  wydarzenie:<br>
  <input type="text" v-model="dat.task"><br>
  temat:<br>
  <input type="text" v-model="dat.topic"><br>
  Powiadomienie:<br>
  <input type="text" v-model="dat.msg"><br>
  <button @click="addtask">Dodaj</button>
</div>
</template>

<script>

export default {
 name:'add-form',
 props:['dat'],
 methods:{
 addtask: function(event){
 this.$emit('addFunction');
 }
 }
}
</script>

<style>
.yes_class
{
 background-color: green;
}

.no_class
{
 background-color: red;
}
</style>