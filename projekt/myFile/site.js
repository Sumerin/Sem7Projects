import myCom from 'com/my_com.vue'

var data = { 
title: 'Moja aplikacja Vue.js',
tab:[ {name:"lol", checked: true}]
};

// Vue.component('my-component',{
	// template: '#com_id',
	// props: ['collection']
// });


new Vue({
	el: '#app',
	data: data
})