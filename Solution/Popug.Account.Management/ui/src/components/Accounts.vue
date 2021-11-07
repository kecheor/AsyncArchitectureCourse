<template>
  <CContainer>
    <div>
      <CCallout v-if="newPopug" color="primary">
        <h2>Create new popug</h2>
        <CForm>
          <div class="mb-3">
            <CFormLabel for="exampleInputEmail1">Chip Id</CFormLabel>
            <CFormInput
              id="chipId"
              aria-describedby="chipHelp"
              v-model="newPopug.chipId"
            />
            <CFormText id="chipHelp"
              >Chip Id should be unique for each popug.</CFormText
            >
          </div>
          <div class="mb-3">
            <CFormLabel for="name">Name</CFormLabel>
            <CFormInput id="name" v-model="newPopug.name" />
          </div>
          <div class="mb-3">
            <CFormLabel for="name">Role</CFormLabel>
            <CFormCheck
              type="radio"
              name="role"
              id="role_user"
              autoComplete="off"
              label="User"
              :checked="newPopug.role == 1"
              @update-model-value="(value) => (newPopug.role = 1)"
            />
            <CFormCheck
              type="radio"
              name="role"
              id="role_manager"
              autoComplete="off"
              label="Manager"
              :checked="newPopug.role == 10"
              @update-model-value="(value) => (newPopug.role = 10)"
            />
            <CFormCheck
              type="radio"
              name="role"
              id="role_admin"
              autoComplete="off"
              label="Admin"
              v-model="newPopug.role"
              :checked="newPopug.role == 100"
              @update-model-value="(value) => (newPopug.role = 100)"
            />
          </div>
          <div class="mb-3">
            <CFormLabel for="name"
              >Beak Curvature: {{ newPopug.beakCurvature }}</CFormLabel
            >
            <CFormRange
              :min="0"
              :max="200"
              value="0"
              id="beakCurvature"
              v-model="newPopug.beakCurvature"
            />
          </div>
          <CButton color="primary" @click="submit"> Create </CButton>
          <CButton color="secondary outline " @click="reset" class="m-1">
            Cancel
          </CButton>
        </CForm>
      </CCallout>
      <CCallout v-else color="primary">
        <CButton type="button" color="primary" @click="create">
          Create
        </CButton>
      </CCallout>
    </div>
    <div>
      <CTable v-if="popugs.length">
        <CTableHead>
          <CTableRow>
            <CTableHeaderCell scope="col">ChipId</CTableHeaderCell>
            <CTableHeaderCell scope="col">Name</CTableHeaderCell>
            <CTableHeaderCell scope="col">Role</CTableHeaderCell>
            <CTableHeaderCell scope="col">Beak Curvature</CTableHeaderCell>
          </CTableRow>
        </CTableHead>
        <CTableBody>
          <CTableRow v-for="popug in popugs" :key="popug.id">
            <CTableHeaderCell scope="row">{{ popug.chipId }}</CTableHeaderCell>
            <CTableDataCell>{{ popug.name }}</CTableDataCell>
            <CTableDataCell>{{ translateRole(popug.role) }}</CTableDataCell>
            <CTableDataCell>{{ popug.beakCurvature }}</CTableDataCell>
          </CTableRow>
        </CTableBody>
      </CTable>
      <CAlert v-else color="secondary">No popugs yet</CAlert>
    </div>
  </CContainer>
</template>

<script lang="ts">
import {
  CContainer,
  CForm,
  CFormLabel,
  CFormInput,
  CFormText,
  CFormCheck,
  CFormRange,
  CButton,
  CCallout,
  CTable,
  CTableHead,
  CTableBody,
  CTableRow,
  CTableHeaderCell,
  CTableDataCell
} from "@coreui/vue";
import { Options, Vue } from "vue-class-component";
import axios from "axios";
class Account {
  id: number | null = null;
  chipId = "";
  name = "";
  role = 1;
  beakCurvature = 0;
}

@Options({
  components: {
    CContainer,
    CForm,
    CFormLabel,
    CFormInput,
    CFormText,
    CFormCheck,
    CFormRange,
    CButton,
    CCallout,
    CTable,
    CTableHead,
    CTableBody,
    CTableRow,
    CTableHeaderCell,
    CTableDataCell
  },
})
export default class Accounts extends Vue {
  popugs: Account[] = [];
  newPopug: Account | null = null;

  async mounted() {
    await this.getAll();
  }

  create(): void {
    this.newPopug = new Account();
  }

  async submit(): Promise<void> {
    await axios.post("api/add", this.newPopug );
    this.newPopug = null;
  }

  reset(): void {
    this.newPopug = null;
  }

  translateRole(roleId: number) {
    switch (roleId) {
      case 1:
        return "User";
      case 10:
        return "Manager";
      case 100:
        return "Admin";
      default:
        return "Unknown";
    }
  }

  private async getAll() {
    this.popugs = (await axios.get<Account[]>("api/all")).data;
  }
}
</script>
<style scoped></style>
