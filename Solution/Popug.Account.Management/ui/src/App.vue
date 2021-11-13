<template>
  <div>
    <CNavbar expand="lg" color-scheme="light" class="bg-light">
      <CContainer fluid>
        <CNavbarBrand>Uber Popug</CNavbarBrand>
        <CNavbarText>{{ popugName }}</CNavbarText>
      </CContainer>
    </CNavbar>
    <Accounts />
  </div>
</template>

<script lang="ts">
import { Options, Vue } from "vue-class-component";
import { CNavbar, CContainer, CNavbarBrand, CNavbarText } from "@coreui/vue";
import Accounts from "./components/Accounts.vue";
import axios from "axios";

@Options({
  components: {
    CNavbar,
    CContainer,
    CNavbarBrand,
    CNavbarText,
    Accounts,
  },
})
export default class App extends Vue {
  popugName = "";

  async created() {
    try {
      var account = await axios.get("/bff/user", { headers: {"X-CSRF": "1" } });
    } catch (e) {
      switch(e.response.status) {
        case 401:
          window.location.href = "/bff/login";
          break;
        default:
          break;
      }
    }
  }
}
</script>

<style>
html,
body,
#app {
  height: 100vh;
}
</style>
