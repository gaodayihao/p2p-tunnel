<template>
    <div class="register-form">
        <div class="inner">
            <el-form label-width="8rem" ref="formDom" :model="state.form" :rules="state.rules">
                <el-form-item label="" label-width="0">
                    <el-row>
                        <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
                            <el-form-item label="P2P加密" prop="ClientEncode">
                                <el-checkbox v-model="state.form.ClientEncode">开启
                                    <el-tooltip class="box-item" effect="dark" content="节点之间通信使用加密" placement="top">
                                        <el-icon>
                                            <Warning />
                                        </el-icon>
                                    </el-tooltip>
                                </el-checkbox>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
                            <el-form-item label="密钥" prop="ClientEncodePassword">
                                <el-input size="default" type="password" show-password maxlength="32" show-word-limit v-model="state.form.ClientEncodePassword" placeholder="加密密钥32位">
                                    <template #append>
                                        <el-tooltip class="box-item" effect="dark" content="加密密钥32位，为空则每次加密随机密钥，如果填写，则各节点都填写" placement="top">
                                            <el-icon>
                                                <Warning />
                                            </el-icon>
                                        </el-tooltip>
                                    </template>
                                </el-input>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form-item>
                <el-form-item label="" label-width="0">
                    <el-row>
                        <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
                            <el-form-item label="登入加密" prop="ServerEncode">
                                <el-checkbox v-model="state.form.ServerEncode">开启
                                    <el-tooltip class="box-item" effect="dark" content="节点与服务端之间通信使用加密" placement="top">
                                        <el-icon>
                                            <Warning />
                                        </el-icon>
                                    </el-tooltip>
                                </el-checkbox>
                            </el-form-item>
                        </el-col>
                        <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
                            <el-form-item label="密钥" prop="ServerEncodePassword">
                                <el-input size="default" type="password" show-password maxlength="32" show-word-limit v-model="state.form.ServerEncodePassword" placeholder="加密密钥32位">
                                    <template #append>
                                        <el-tooltip class="box-item" effect="dark" content="加密密钥32位，为空则每次加密随机密钥，使用默认服务器则必须留空" placement="top">
                                            <el-icon>
                                                <Warning />
                                            </el-icon>
                                        </el-tooltip>
                                    </template>
                                </el-input>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-form-item>
            </el-form>
        </div>
    </div>
</template>

<script>
import { ref, reactive } from '@vue/reactivity';
import { getSignInInfo, updateConfig } from '../../../apis/signin'
import { onMounted } from '@vue/runtime-core';
import plugin from './plugin'
export default {
    plugin: Object.assign(JSON.parse(JSON.stringify(plugin)), { text: '加密配置', order: -97 }),
    setup() {
        const formDom = ref(null);
        const state = reactive({
            form: {
                ClientEncode: false,
                ClientEncodePassword: "",
                ServerEncode: false,
                ServerEncodePassword: "",
            },
            rules: {
            }
        });

        const loadConfig = () => {
            getSignInInfo().then((json) => {
                state.form.ClientEncode = json.ClientConfig.Encode;
                state.form.ClientEncodePassword = json.ClientConfig.EncodePassword;

                state.form.ServerEncode = json.ServerConfig.Encode;
                state.form.ServerEncodePassword = json.ServerConfig.EncodePassword;
            }).catch((msg) => {
            });
        }
        const getJson = () => {
            return new Promise((resolve, reject) => {
                getSignInInfo().then((json) => {
                    json.ClientConfig.Encode = state.form.ClientEncode;
                    json.ClientConfig.EncodePassword = state.form.ClientEncodePassword;
                    json.ServerConfig.Encode = state.form.ServerEncode;
                    json.ServerConfig.EncodePassword = state.form.ServerEncodePassword;
                    resolve(json);
                }).catch(reject);
            });
        }
        const submit = () => {
            return new Promise((resolve, reject) => {
                formDom.value.validate((valid) => {
                    if (valid == false) {
                        reject();
                        return false;
                    }
                    getJson().then((json) => {
                        updateConfig(json).then(resolve).catch(reject);
                    }).catch(reject);
                });
            });
        }

        onMounted(() => {
            loadConfig();
        });

        return {
            state, formDom, submit
        }
    }
}
</script>

<style lang="stylus" scoped>
.el-row {
    width: 100%;
}

.el-form-item:last-child {
    margin-bottom: 0;
}

@media screen and (max-width: 768px) {
    .el-col {
        margin-top: 0.6rem;
    }
}
</style>