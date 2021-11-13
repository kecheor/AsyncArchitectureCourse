import { createApp } from "vue";
import App from "./App.vue";
import "@coreui/coreui/dist/css/coreui.min.css";
import CoreuiVue from '@coreui/vue';

const app = createApp(App);
app.mount("#app");
app.use(CoreuiVue);
