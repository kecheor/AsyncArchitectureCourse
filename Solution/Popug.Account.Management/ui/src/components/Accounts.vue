<template>
  <CContainer>
    <div v-if="popug">
      <CCallout color="primary">
        <h2>Create new popug</h2>
        <div class="mb-3">
          <CFormLabel for="exampleInputEmail1">Chip Id</CFormLabel>
          <CFormInput
            id="chipId"
            aria-describedby="chipHelp"
            v-model="popug.chipId"
          />
          <CFormText id="chipHelp"
            >Chip Id should be unique for each popug.</CFormText
          >
        </div>
        <div class="mb-3">
          <CFormLabel for="name">Name</CFormLabel>
          <CFormInput id="name" v-model="popug.name" />
        </div>
        <div class="mb-3">
          <CFormLabel for="name">Role</CFormLabel>
          <CFormCheck
            type="radio"
            name="role"
            id="role_user"
            label="User"
            :checked="popug.role == 1"
            @update-model-value="(value) => (popug.role = 1)"
          />
          <CFormCheck
            type="radio"
            name="role"
            id="role_manager"
            autoComplete="off"
            label="Manager"
            :checked="popug.role == 10"
            @update-model-value="(value) => (popug.role = 10)"
          />
          <CFormCheck
            type="radio"
            name="role"
            id="role_admin"
            autoComplete="off"
            label="Admin"
            v-model="popug.role"
            :checked="popug.role == 100"
            @update-model-value="(value) => (popug.role = 100)"
          />
        </div>
        <div class="mb-3">
          <CFormLabel for="name"
            >Beak Curvature: {{ popug.beakCurvature }}</CFormLabel
          >
          <CFormRange
            :min="0"
            :max="200"
            value="0"
            id="beakCurvature"
            v-model="popug.beakCurvature"
          />
        </div>
        <CButton color="primary" @click="post"> Create </CButton>
        <CButton color="secondary outline " @click="reset" class="m-1">
          Cancel
        </CButton>
      </CCallout>
    </div>
    <div v-else>
      <CCallout color="primary">
        <CButton type="button" color="primary" @click="create">
          Create
        </CButton>
      </CCallout>
      <CTable v-if="popugs.length">
        <CTableHead>
          <CTableRow>
            <CTableHeaderCell scope="col">ChipId</CTableHeaderCell>
            <CTableHeaderCell scope="col">Name</CTableHeaderCell>
            <CTableHeaderCell scope="col">Role</CTableHeaderCell>
            <CTableHeaderCell scope="col">Beak Curvature</CTableHeaderCell>
            <CTableHeaderCell scope="col"></CTableHeaderCell>
          </CTableRow>
        </CTableHead>
        <CTableBody>
          <CTableRow v-for="popug in popugs" :key="popug.id">
            <CTableHeaderCell scope="row">{{ popug.chipId }}</CTableHeaderCell>
            <CTableDataCell>{{ popug.name }}</CTableDataCell>
            <CTableDataCell>{{ translateRole(popug.role) }}</CTableDataCell>
            <CTableDataCell>{{ popug.beakCurvature }}</CTableDataCell>
            <CTableDataCell>
              <CButton type="button" color="primary" @click="edit(popug)">
                Edit
              </CButton>
            </CTableDataCell>
          </CTableRow>
        </CTableBody>
      </CTable>
      <CAlert v-else color="secondary">No popugs yet</CAlert>
    </div>
  </CContainer>
</template>

<script lang="ts">
import { Options, Vue } from "vue-class-component";
import axios from "axios";
class Account {
  id: number | null = null;
  chipId = "";
  name = "";
  role = 1;
  beakCurvature = 0;
}

@Options({})
export default class Accounts extends Vue {
  popugs: Account[] = [];
  popug: Account | null = null;

  async mounted() {
    await this.getAll();
  }

  create(): void {
    this.popug = new Account();
  }

  edit(p: Account): void {
    this.popug = p;
  }

  async post(): Promise<void> {
    if(this.popug?.id)
    {
      await axios.post("api/update", this.popug, { headers: { "X-CSRF": "1" } });
    }
    else if(this.popug)
    {
      await axios.post("api/add", this.popug, { headers: { "X-CSRF": "1" } });
    }
    this.popug = null;
  }

  reset(): void {
    this.popug = null;
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
    this.popugs = (
      await axios.get<Account[]>("api/all", { headers: { "X-CSRF": "1" } })
    ).data;
  }
}
</script>
<style scoped></style>
