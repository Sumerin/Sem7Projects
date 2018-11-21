import Vue from 'vue'
import App from './App.vue'

Vue.filter('filterClock' , (value) =>
{
	return value.hour + ":" + value.min + ":" + value.sec;
})

new Vue({
  el: '#app',
  render: h => h(App)
})

