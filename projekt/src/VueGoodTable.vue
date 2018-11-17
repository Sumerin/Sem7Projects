<template>
  <div>
  	<h2>
	Harmonogram:
	<my-clock :collection='time'></my-clock>
	</h2>
	<vue-good-table :columns="columns"
					:rows="rows"
					:paginate="true"
					:lineNumbers="true"
					:globalSearch="true" >
	  </vue-good-table>
  </div>
</template>

<script>

import clock from './clock.vue'
import { VueGoodTable } from 'vue-good-table'
import 'vue-good-table/dist/vue-good-table.css'
export default {
 name:'my-table',
 props:['rows','time'],
 data ()
 {
 return {
 columns:[
	{label:'Czas',field:'time'},
	{label:'Zajecie',field:'task'},
	{label:'Tematyka',field:'topic'}
	]
	}
 },
 components: {
	'my-clock':clock,
	'vue-good-table':VueGoodTable,
}
}
</script>